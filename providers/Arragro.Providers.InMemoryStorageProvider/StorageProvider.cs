using Arragro.Core.Common.CacheProvider;
using Arragro.Core.Common.Interfaces.Providers;
using Arragro.Core.Common.Models;
using Arragro.Providers.InMemoryStorageProvider.FileInfo;
using Microsoft.Extensions.FileProviders;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Arragro.Providers.InMemoryStorageProvider
{
    public class StorageProvider<FolderIdType, FileIdType> : IStorageProvider<FolderIdType, FileIdType>
    {
        private readonly IImageProvider _imageService;
        private readonly InMemoryFileProvider _provider;

        public StorageProvider(
            IImageProvider imageProcessor)
        {
            _imageService = imageProcessor;
            _provider = new InMemoryFileProvider();
        }

        const string THUMBNAIL_ASSETKEY = "ThumbNail:";
        const string ASSET_ASSETKEY = "Asset:";
        const string ASSET_QUALITY_ASSETKEY = "Asset:Quality:";
        const string ASSET_QUALITY_WIDTH_ASSETKEY = "Asset:Quality:Width:";

        public void ClearCache()
        {
            Cache.RemoveFromCache($"{THUMBNAIL_ASSETKEY}.*", true);
            Cache.RemoveFromCache($"{ASSET_ASSETKEY}.*", true);
            Cache.RemoveFromCache($"{ASSET_QUALITY_ASSETKEY}.*", true);
            Cache.RemoveFromCache($"{ASSET_QUALITY_WIDTH_ASSETKEY}.*", true);
        }

        public async Task<bool> Delete(FolderIdType folderId, FileIdType fileId, bool thumbNail = false)
        {
            return await Task.Run(() =>
            {
                var thumbNails = thumbNail ? "thumbnails/" : "";
                var fileInfo = _provider.Directory.GetFile($"/{folderId}/{thumbNails}{fileId}");
                if (fileInfo == null)
                    return false;

                fileInfo.Delete();
                return true;
            });
        }

        private async Task DeleteFolder(string folder)
        {
            await Task.Run(() =>
            {
                var directory = _provider.Directory.GetFolder(folder);
                directory.Delete();
            });
        }

        public async Task Delete(FolderIdType folderId)
        {
            await DeleteFolder($"/{folderId}/thumbnails");
            await DeleteFolder($"/{folderId}");
        }

        private async Task<Uri> Get(FolderIdType folderId, FileIdType fileId)
        {
            var fileInfo = _provider.GetFileInfo($"/{folderId}/{fileId}");

            if (fileInfo != null)
            {
                return await Task.Run(() =>
                {
                    return new Uri($"http://inmemoryfileprovider.com/{folderId}/{fileId}");
                });
            }

            return null;
        }

        private async Task<Uri> GetImageThumbnail(FolderIdType folderId, FileIdType fileId)
        {
            var fileInfo = _provider.GetFileInfo($"/{folderId}/thumbnails/{fileId}");

            if (fileInfo != null)
            {
                return await Task.Run(() =>
                {
                    return new Uri($"http://inmemoryfileprovider.com/{folderId}/{fileId}");
                });
            }

            return null;
        }

        private async Task<byte[]> GetImageBytes(IFileInfo fileInfo)
        {
            using (var ms = new MemoryStream())
            {
                await fileInfo.CreateReadStream().CopyToAsync(ms);
                return ms.ToArray();
            }
        }

        public async Task<Uri> Upload(FolderIdType folderId, FileIdType fileId, byte[] data, string mimeType)
        {
            await Task.Run(() =>
            {
                using (var ms = new MemoryStream(data))
                {
                    ms.Position = 0;
                    _provider.Directory.AddFile($"/{folderId}", new MemoryStreamFileInfo(ms, Encoding.UTF8, fileId.ToString()));
                }
            });
            return new Uri($"http://inmemoryfileprovider.com/{folderId}/{fileId}");
        }

        private async Task Upload(FolderIdType folderId, FileIdType fileId, int quality, int width, byte[] data, string mimeType)
        {
            await Task.Run(() =>
            {
                using (var ms = new MemoryStream(data))
                {
                    ms.Position = 0;
                    _provider.Directory.AddFile($"/{folderId}/{fileId}/{quality}", new MemoryStreamFileInfo(ms, Encoding.UTF8, width.ToString()));
                }
            });
        }

        public async Task<Uri> UploadThumbnail(FolderIdType folderId, FileIdType fileId, byte[] data, string mimeType)
        {
            await Task.Run(() =>
            {
                using (var ms = new MemoryStream(data))
                {
                    ms.Position = 0;
                    _provider.Directory.AddFile($"/{folderId}/thumbnails", new MemoryStreamFileInfo(ms, Encoding.UTF8, fileId.ToString()));
                }
            });

            return new Uri($"http://inmemoryfileprovider.com/{folderId}/thumbnails/{fileId}");
        }
        
        public async Task ResetCacheControl()
        {
            await Task.Run(() =>
            {
                return;
            });
        }

        public async Task<Uri> Get(FolderIdType folderId, FileIdType fileId, bool thumbnail = false)
        {
            if (thumbnail)
                return await GetImageThumbnail(folderId, fileId);
            return await Get(folderId, fileId);
        }

        public async Task<CreateImageFromImageResult> CreateImageFromExistingImage(FolderIdType folderId, FileIdType fileId, FileIdType newFileId)
        {
            var fileName = $"{folderId}/{fileId}";
            var fileInfo = _provider.GetFileInfo(fileName);
            var bytes = await GetImageBytes(fileInfo);

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
            var uri = await Upload(folderId, newFileId, bytes, "");
            imageResult = await _imageService.ResizeAndProcessImageAsync(bytes, 250, 60, true);
            var thumbnailUri = await Upload(folderId, newFileId, imageResult.Bytes, "", true);

            return new CreateImageFromImageResult
            {
                ImageProcessResult = imageResult,
                Uri = uri,
                ThumbnailUri = thumbnailUri
            };
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

            var fileInfo = _provider.GetFileInfo(fileName);
            var bytes = await GetImageBytes(fileInfo);

            await Delete(folderId, fileId, thumbnail);
            return await Upload(folderId, newFileId, bytes, "", thumbnail);
        }
    }
}
