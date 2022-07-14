using Arragro.Core.Common.CacheProvider;
using Arragro.Core.Common.Interfaces.Providers;
using Arragro.Core.Common.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.IO;
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

        public async Task<bool> DeleteAsync(FolderIdType folderId, FileIdType fileId, bool thumbNail = false)
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

        protected async Task DeleteFolderAsync(string folder)
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

        public async Task DeleteAsync(FolderIdType folderId)
        {
            await DeleteFolderAsync($"{folderId}/thumbnails");
            await DeleteFolderAsync($"{folderId}");
        }

        protected async Task<Uri> GetAsync(FolderIdType folderId, FileIdType fileId)
        {
            var key = $"{ASSET_ASSETKEY}{folderId}:{fileId}";
            var cacheItem = CacheProviderManager.CacheProvider.Get<Uri>(key);
            if (cacheItem != null && cacheItem.Item != null)
                return cacheItem.Item;

            var blob = _assetContainerClient.GetBlobClient($"{folderId}/{fileId}");

            if (await blob.ExistsAsync())
            {
                CacheProviderManager.CacheProvider.Set(key, blob.Uri, new Arragro.Core.Common.CacheProvider.CacheSettings(new TimeSpan(0, 30, 0), true));
                return blob.Uri;
            }

            return null;
        }

        protected async Task<Uri> UploadAsync(FolderIdType folderId, FileIdType fileId, byte[] data, string mimeType)
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

        protected async Task<Uri> GetImageThumbnailAsync(FolderIdType folderId, FileIdType fileId)
        {
            var key = $"{THUMBNAIL_ASSETKEY}{folderId}:{fileId}";
            var cacheItem = CacheProviderManager.CacheProvider.Get<Uri>(key);
            if (cacheItem != null && cacheItem.Item != null)
                return cacheItem.Item;

            var blob = _assetContainerClient.GetBlobClient($"{folderId}/thumbnails/{fileId}");
            if (await blob.ExistsAsync())
            {
                CacheProviderManager.CacheProvider.Set(key, blob.Uri, new Arragro.Core.Common.CacheProvider.CacheSettings(new TimeSpan(0, 30, 0), true));
                return blob.Uri;
            }

            return null;
        }

        public async Task ResetCacheControlAsync()
        {
            var resultSegment = _assetContainerClient.GetBlobsAsync()
                .AsPages(default);

            await foreach (Azure.Page<BlobItem> blobPage in resultSegment)
            {
                foreach (BlobItem blobItem in blobPage.Values)
                {
                    await this.ResetCloudBlobCacheControlAsync(blobItem.Name, _cacheControlMaxAge);
                }
            }
        }

        protected async Task ResetCloudBlobCacheControlAsync(string blobName, int cacheControlMaxAge)
        {
            var blobClient = _assetContainerClient.GetBlobClient(blobName);
            var properties = await blobClient.GetPropertiesAsync();
            var httpHeaders = GetBlobHttpHeaders(properties);
            httpHeaders.CacheControl = $"public, max-age={cacheControlMaxAge}";
            await blobClient.SetHttpHeadersAsync(httpHeaders);
        }

        protected async Task<Uri> UploadThumbnailAsync(FolderIdType folderId, FileIdType fileId, byte[] data, string mimeType)
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

        public async Task<Uri> GetAsync(FolderIdType folderId, FileIdType fileId, bool thumbnail = false)
        {
            if (thumbnail)
                return await GetImageThumbnailAsync(folderId, fileId);
            return await GetAsync(folderId, fileId);
        }

        public async Task<CreateAssetFromExistingResult> CreateAssetFromExistingAsync(FolderIdType folderId, FileIdType fileId, FileIdType newFileId)
        {
            var fileName =$"{folderId}/{fileId}";
            var newFileName =$"{folderId}/{newFileId}";

            var blobCopy = _assetContainerClient.GetBlobClient(newFileName);
            if (!await blobCopy.ExistsAsync())
            {
                var blob = _assetContainerClient.GetBlobClient(fileName);

                if (await blob.ExistsAsync())
                {
                    byte[] bytes;

                    using (var ms = new MemoryStream())
                    {
                        var download = await blob.DownloadAsync();
                        await download.Value.Content.CopyToAsync(ms);
                        bytes = ms.ToArray();
                    }

                    var properties = await blob.GetPropertiesAsync();
                    var imageProcessDetailsResult = await _imageService.GetImageDetailsAsync(bytes);
                    var imageResult = new ImageProcessResult
                    {
                        Height = imageProcessDetailsResult.Height,
                        Width = imageProcessDetailsResult.Width,
                        IsImage = imageProcessDetailsResult.IsImage,
                        MimeType = imageProcessDetailsResult.MimeType,
                        Size = imageProcessDetailsResult.Size,
                        Bytes = bytes
                    };
                    var uri = await UploadAsync(folderId, newFileId, bytes, properties.Value.ContentType);
                    var thumbNailImageResult = await _imageService.ResizeAndProcessImageAsync(bytes, 250, 60, true);
                    Uri thumbnailUri = null;
                    if (imageResult.IsImage)
                    {
                        thumbnailUri = await UploadAsync(folderId, newFileId, thumbNailImageResult.Bytes, properties.Value.ContentType, true);
                    }

                    return new CreateAssetFromExistingResult
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

        public async Task<CreateAssetFromExistingResult> CreateAssetFromExistingAndResizeAsync(FolderIdType folderId, FileIdType fileId, FileIdType newFileId, int width, int quality = 80, bool asProgressiveJpeg = false)
        {
            var fileName = $"{folderId}/{fileId}";
            var newFileName = $"{folderId}/{newFileId}";

            var blobCopy = _assetContainerClient.GetBlobClient(newFileName);
            if (!await blobCopy.ExistsAsync())
            {
                var blob = _assetContainerClient.GetBlobClient(fileName);

                if (await blob.ExistsAsync())
                {
                    byte[] bytes;

                    using (var ms = new MemoryStream())
                    {
                        var download = await blob.DownloadAsync();
                        await download.Value.Content.CopyToAsync(ms);
                        bytes = ms.ToArray();
                    }

                    var properties = await blob.GetPropertiesAsync();
                    var imageProcessResult = await _imageService.ResizeAndProcessImageAsync(bytes, width, quality, asProgressiveJpeg);
                    var uri = await UploadAsync(folderId, newFileId, imageProcessResult.Bytes, properties.Value.ContentType);
                    var thumbNailImageResult = await _imageService.ResizeAndProcessImageAsync(imageProcessResult.Bytes, 250, 60, true);
                    Uri thumbnailUri = null;
                    if (imageProcessResult.IsImage)
                    {
                        thumbnailUri = await UploadAsync(folderId, newFileId, thumbNailImageResult.Bytes, properties.Value.ContentType, true);
                    }

                    return new CreateAssetFromExistingResult
                    {
                        ImageProcessResult = imageProcessResult,
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

        public async Task<Uri> UploadAsync(FolderIdType folderId, FileIdType fileId, byte[] data, string mimeType, bool thumbnail = false)
        {
            if (thumbnail)
                return await UploadThumbnailAsync(folderId, fileId, data, mimeType);
            return await UploadAsync(folderId, fileId, data, mimeType);
        }

        public async Task<Uri> RenameAsync(FolderIdType folderId, FileIdType fileId, FileIdType newFileId, bool thumbnail = false)
        {
            var fileName = thumbnail ? $"{folderId}/thumbnails/{fileId}" : $"{folderId}/{fileId}";
            var newFileName = thumbnail ? $"{folderId}/thumbnails/{newFileId}" : $"{folderId}/{newFileId}";

            var blobCopy = _assetContainerClient.GetBlobClient(newFileName);
            if (!await blobCopy.ExistsAsync())
            {
                var blob = _assetContainerClient.GetBlobClient(fileName);

                if (await blob.ExistsAsync())
                {
                    await blobCopy.StartCopyFromUriAsync(blob.Uri);
                    await blob.DeleteIfExistsAsync();
                }
            }
            return blobCopy.Uri;
        }
    }
}
