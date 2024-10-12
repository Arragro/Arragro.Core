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

        public async Task<bool> DeleteAsync(FolderIdType folderId, FileIdType fileId, bool thumbNail = false)
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

        private async Task DeleteFolderAsync(string folder)
        {
            await Task.Run(() =>
            {
                var directory = _provider.Directory.GetFolder(folder);
                directory.Delete();
            });
        }

        public async Task DeleteAsync(FolderIdType folderId)
        {
            await DeleteFolderAsync($"/{folderId}/thumbnails");
            await DeleteFolderAsync($"/{folderId}");
        }

        private async Task<Uri> GetAsync(FolderIdType folderId, FileIdType fileId)
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

        private async Task<Uri> GetImageThumbnailAsync(FolderIdType folderId, FileIdType fileId)
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

        private async Task<byte[]> GetImageBytesAsync(IFileInfo fileInfo)
        {
            using (var ms = new MemoryStream())
            {
                await fileInfo.CreateReadStream().CopyToAsync(ms);
                return ms.ToArray();
            }
        }

        public async Task<Uri> UploadAsync(FolderIdType folderId, FileIdType fileId, byte[] data, string mimeType)
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

        private async Task UploadAsync(FolderIdType folderId, FileIdType fileId, int quality, int width, byte[] data, string mimeType)
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

        public async Task<Uri> UploadThumbnailAsync(FolderIdType folderId, FileIdType fileId, byte[] data, string mimeType)
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
        
        public async Task ResetCacheControlAsync()
        {
            await Task.Run(() =>
            {
                return;
            });
        }

        public async Task<Uri> GetAsync(FolderIdType folderId, FileIdType fileId, bool thumbnail = false)
        {
            if (thumbnail)
                return await GetImageThumbnailAsync(folderId, fileId);
            return await GetAsync(folderId, fileId);
        }

        public async Task<CreateAssetFromExistingResult> CreateAssetFromExistingAsync(FolderIdType folderId, FileIdType fileId, FileIdType newFileId)
        {
            var fileName = $"{folderId}/{fileId}";
            var fileInfo = _provider.GetFileInfo(fileName);
            var bytes = await GetImageBytesAsync(fileInfo);

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
            var uri = await UploadAsync(folderId, newFileId, bytes, imageProcessDetailsResult.MimeType);
            imageResult = await _imageService.ResizeAndProcessImageAsync(bytes, 250, 60, true);
            Uri thumbnailUri = null;
            if (imageResult.IsImage)
            {
                thumbnailUri = await UploadAsync(folderId, newFileId, imageResult.Bytes, "", true);
            }

            return new CreateAssetFromExistingResult
            {
                ImageProcessResult = imageResult,
                Uri = uri,
                ThumbnailUri = thumbnailUri
            };
        }

        public async Task<CreateAssetFromExistingResult> CreateAssetFromExistingAndResizeAsync(FolderIdType folderId, FileIdType fileId, FileIdType newFileId, uint width, uint quality = 80, bool asProgressiveJpeg = false)
        {
            var fileName = $"{folderId}/{fileId}";
            var fileInfo = _provider.GetFileInfo(fileName);
            var bytes = await GetImageBytesAsync(fileInfo);

            var imageProcessResult = await _imageService.ResizeAndProcessImageAsync(bytes, width, quality, asProgressiveJpeg);
            var uri = await UploadAsync(folderId, newFileId, imageProcessResult.Bytes, imageProcessResult.MimeType);
            var thumbnailResult = await _imageService.ResizeAndProcessImageAsync(bytes, 250, 60, true);
            Uri thumbnailUri = null;
            if (thumbnailResult.IsImage)
            {
                thumbnailUri = await UploadAsync(folderId, newFileId, thumbnailResult.Bytes, "", true);
            }

            return new CreateAssetFromExistingResult
            {
                ImageProcessResult = imageProcessResult,
                Uri = uri,
                ThumbnailUri = thumbnailUri
            };
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

            var fileInfo = _provider.GetFileInfo(fileName);
            var bytes = await GetImageBytesAsync(fileInfo);

            await DeleteAsync(folderId, fileId, thumbnail);
            return await UploadAsync(folderId, newFileId, bytes, "", thumbnail);
        }
    }
}
