using Arragro.Core.Common.CacheProvider;
using Arragro.Core.Common.Interfaces.Providers;
using Arragro.Core.Common.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Arragro.Providers.AzureStorageProvider
{
    public static class StorageProviderExtentions
    {
        public static async Task ConfigureAzureStorageProvider( 
            string storageConnectionString,
            string assetsContainerName = "assets")
        {
            var assetContainerClient = new BlobContainerClient(storageConnectionString, assetsContainerName);
            await assetContainerClient.CreateIfNotExistsAsync();
            await assetContainerClient.SetAccessPolicyAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob);
        }
    }

    public class StorageProvider<FolderIdType, FileIdType> : IStorageProvider<FolderIdType, FileIdType>
    {
        protected readonly IImageProvider _imageService;

        protected readonly string _storageConnectionString;
        protected readonly int _cacheControlMaxAge;

        protected readonly BlobContainerClient _assetContainerClient;

        public StorageProvider(
            IImageProvider imageProcessor,
            string storageConnectionString,
            int cacheControlMaxAge = 0,
            string assetsContainerName = "assets")
        {
            _imageService = imageProcessor;
            _storageConnectionString = storageConnectionString;
            _cacheControlMaxAge = cacheControlMaxAge;
            _assetContainerClient = new BlobContainerClient(storageConnectionString, assetsContainerName);
        }

        protected const string THUMBNAIL_ASSETKEY = "ThumbNail:";
        protected const string ASSET_ASSETKEY = "Asset:";
        protected const string ASSET_QUALITY_ASSETKEY = "Asset:Quality:";
        protected const string ASSET_QUALITY_WIDTH_ASSETKEY = "Asset:Quality:Width:";

        public async Task<bool> Delete(FolderIdType folderId, FileIdType fileId, bool thumbNail = false)
        {
            var thumbNails = thumbNail ? "thumbnails/" : "";
            var blob = _assetContainerClient.GetBlobClient($"{folderId}/{thumbNails}{fileId}");
            return await blob.DeleteIfExistsAsync();
        }

        private BlobHttpHeaders GetBlobHttpHeaders(BlobProperties blobProperties)
        {
            return new BlobHttpHeaders
            {
                ContentType = blobProperties.ContentType,
                CacheControl = blobProperties.CacheControl
            };
        }

        protected async Task DeleteFolder(string folder)
        {
            var resultSegment = _assetContainerClient.GetBlobsAsync(prefix: folder)
                .AsPages(default);

            await foreach (Azure.Page<BlobItem> blobPage in resultSegment)
            {
                foreach (BlobItem blobItem in blobPage.Values)
                {
                    await _assetContainerClient.DeleteBlobIfExistsAsync(blobItem.Name);
                }
            }
        }

        public async Task Delete(FolderIdType folderId)
        {
            await DeleteFolder($"{folderId}/thumbnails");
            await DeleteFolder($"{folderId}");
        }

        protected async Task<Uri> Get(FolderIdType folderId, FileIdType fileId)
        {
            var key = $"{ASSET_ASSETKEY}{folderId}:{fileId}";
            var cacheItem = CacheProviderManager.CacheProvider.Get<Uri>(key);
            if (cacheItem != null && cacheItem.Item != null)
                return cacheItem.Item;

            var blob = _assetContainerClient.GetBlobClient($"{folderId}/{fileId}");

            if (await blob.ExistsWorkAroundAsync())
            {
                CacheProviderManager.CacheProvider.Set(key, blob.Uri, new Arragro.Core.Common.CacheProvider.CacheSettings(new TimeSpan(0, 30, 0), true));
                return blob.Uri;
            }

            return null;
        }

        protected async Task<Uri> Upload(FolderIdType folderId, FileIdType fileId, byte[] data, string mimeType)
        {
            var blob = _assetContainerClient.GetBlobClient($"{folderId}/{fileId}");
            using (var stream = new MemoryStream(data))
            {
                await blob.UploadAsync(stream, new BlobHttpHeaders 
                { 
                    ContentType = mimeType,
                    CacheControl = $"public, max-age={_cacheControlMaxAge}"
                });
                return blob.Uri;
            }
        }

        protected async Task<Uri> GetImageThumbnail(FolderIdType folderId, FileIdType fileId)
        {
            var key = $"{THUMBNAIL_ASSETKEY}{folderId}:{fileId}";
            var cacheItem = CacheProviderManager.CacheProvider.Get<Uri>(key);
            if (cacheItem != null && cacheItem.Item != null)
                return cacheItem.Item;

            var blob = _assetContainerClient.GetBlobClient($"{folderId}/thumbnails/{fileId}");
            if (await blob.ExistsWorkAroundAsync())
            {
                CacheProviderManager.CacheProvider.Set(key, blob.Uri, new Arragro.Core.Common.CacheProvider.CacheSettings(new TimeSpan(0, 30, 0), true));
                return blob.Uri;
            }

            return null;
        }

        public async Task ResetCacheControl()
        {
            var resultSegment = _assetContainerClient.GetBlobsAsync()
                .AsPages(default);

            await foreach (Azure.Page<BlobItem> blobPage in resultSegment)
            {
                foreach (BlobItem blobItem in blobPage.Values)
                {
                    await this.ResetCloudBlobCacheControl(blobItem.Name, _cacheControlMaxAge);
                }
            }
        }

        protected async Task ResetCloudBlobCacheControl(string blobName, int cacheControlMaxAge)
        {
            var blobClient = _assetContainerClient.GetBlobClient(blobName);
            var properties = await blobClient.GetPropertiesAsync();
            var httpHeaders = GetBlobHttpHeaders(properties);
            httpHeaders.CacheControl = $"public, max-age={cacheControlMaxAge}";
            await blobClient.SetHttpHeadersAsync(httpHeaders);
        }

        protected async Task<Uri> UploadThumbnail(FolderIdType folderId, FileIdType fileId, byte[] data, string mimeType)
        {
            var blob = _assetContainerClient.GetBlobClient($"{folderId}/thumbnails/{fileId}");
            using (var stream = new MemoryStream(data))
            {
                await blob.UploadAsync(stream, new BlobHttpHeaders
                {
                    ContentType = mimeType,
                    CacheControl = $"public, max-age={_cacheControlMaxAge}"
                });
                return blob.Uri;
            }
        }

        public async Task<Uri> Get(FolderIdType folderId, FileIdType fileId, bool thumbnail = false)
        {
            if (thumbnail)
                return await GetImageThumbnail(folderId, fileId);
            return await Get(folderId, fileId);
        }

        public async Task<CreateImageFromImageResult> CreateImageFromExistingImage(FolderIdType folderId, FileIdType fileId, FileIdType newFileId, int quality, int width, bool asProgressive = true)
        {
            var fileName =$"{folderId}/{fileId}";
            var newFileName =$"{folderId}/{newFileId}";

            var blobCopy = _assetContainerClient.GetBlobClient(newFileName);
            if (!await blobCopy.ExistsWorkAroundAsync())
            {
                var blob = _assetContainerClient.GetBlobClient(fileName);

                if (await blob.ExistsWorkAroundAsync())
                {
                    byte[] bytes;

                    using (var ms = new MemoryStream())
                    {
                        var download = await blob.DownloadAsync();
                        await download.Value.Content.CopyToAsync(ms);
                        bytes = ms.ToArray();
                    }

                    var properties = await blob.GetPropertiesAsync();
                    var imageResult = await _imageService.GetImage(bytes, width, quality, asProgressive);
                    var uri = await Upload(folderId, newFileId, imageResult.Bytes, properties.Value.ContentType);
                    var thumbNailImageResult = await _imageService.GetImage(bytes, 250, 60, true);
                    var thumbnailUri = await Upload(folderId, newFileId, thumbNailImageResult.Bytes, properties.Value.ContentType, true);

                    return new CreateImageFromImageResult
                    {
                        ImageProcessResult = imageResult,
                        Uri = uri,
                        ThumbnailUri = thumbnailUri
                    };
                }
                else
                {
                    throw new Exception($"The blob you want to copy doesn't exists - {blob.Uri}!");
                }
            }
            else
            {
                throw new Exception($"The blob you want to create already exists - {blobCopy.Uri}!");
            }
        }

        public async Task<Uri> Upload(FolderIdType folderId, FileIdType fileId, byte[] data, string mimeType, bool thumbnail = false)
        {
            if (thumbnail)
                return await UploadThumbnail(folderId, fileId, data, mimeType);
            return await Upload(folderId, fileId, data, mimeType);
        }

        public async Task<Uri> Rename(FolderIdType folderId, FileIdType fileId, FileIdType newFileId, bool thumbnail = false)
        {
            var fileName = thumbnail ? $"{folderId}/thumbnails/{fileId}" : $"{folderId}/{fileId}";
            var newFileName = thumbnail ? $"{folderId}/thumbnails/{newFileId}" : $"{folderId}/{newFileId}";

            var blobCopy = _assetContainerClient.GetBlobClient(newFileName);
            if (!await blobCopy.ExistsWorkAroundAsync())
            {
                var blob = _assetContainerClient.GetBlobClient(fileName);

                if (await blob.ExistsWorkAroundAsync())
                {
                    await blobCopy.StartCopyFromUriAsync(blob.Uri);
                    await blob.DeleteIfExistsAsync();
                }
            }
            return blobCopy.Uri;
        }
    }
}
