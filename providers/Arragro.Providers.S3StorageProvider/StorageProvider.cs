using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Arragro.Core.Common.CacheProvider;
using Arragro.Core.Common.Interfaces.Providers;
using Arragro.Core.Common.Models;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Arragro.Providers.S3StorageProvider
{
    public static class AWSHelper
    {
        public static AmazonS3Client BuildAmazonS3Client(
            string accessKey,
            string secretKey,
            RegionEndpoint regionEndpoint,
            string minioServerUrl = null)
        {
            var credentials = new BasicAWSCredentials(accessKey, secretKey);

            var config = new AmazonS3Config
            {
                RegionEndpoint = regionEndpoint,
            };

            if (!string.IsNullOrEmpty(minioServerUrl))
            {
                config.ServiceURL = minioServerUrl;
                config.ForcePathStyle = true;
            }

            return new AmazonS3Client(credentials, config);
        }

        public static async Task ConfigureAzureStorageProvider(
            AmazonS3Client amazonS3Client,
            string bucketName)
        {
            if (!(await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(amazonS3Client, bucketName)))
            {
                await amazonS3Client.PutBucketAsync(bucketName);
            }
        }
    }


    public class StorageProvider<FolderIdType, FileIdType> : IStorageProvider<FolderIdType, FileIdType>
    {
        protected readonly IImageProvider _imageService;

        protected readonly int _cacheControlMaxAge;
        protected readonly string _bucketName;
        protected readonly string _prefix;
        protected readonly string _minioServerUrl;

        protected readonly BasicAWSCredentials _credentials;
        private readonly ILogger<StorageProvider<FolderIdType, FileIdType>> _logger;
        protected readonly AmazonS3Client _client;
        protected readonly RegionEndpoint _regionEndpoint;

        public StorageProvider(
            ILogger<StorageProvider<FolderIdType, FileIdType>> logger,
            AmazonS3Client amazonS3Client,
            IImageProvider imageProcessor,
            RegionEndpoint regionEndpoint,
            string bucketName,
            int cacheControlMaxAge = 0,
            string prefix = "assets",
            string minioServerUrl = null)
        {
            _logger = logger;
            _imageService = imageProcessor;

            _minioServerUrl = minioServerUrl;
            _regionEndpoint = regionEndpoint;
            _client = amazonS3Client;

            _bucketName = bucketName;
            _cacheControlMaxAge = cacheControlMaxAge;
            _prefix = string.IsNullOrEmpty(prefix) ? "assets" : prefix;
        }

        protected const string THUMBNAIL_ASSETKEY = "ThumbNail:";
        protected const string ASSET_ASSETKEY = "Asset:";
        protected const string ASSET_QUALITY_ASSETKEY = "Asset:Quality:";
        protected const string ASSET_QUALITY_WIDTH_ASSETKEY = "Asset:Quality:Width:";

        public async Task<bool> DeleteAsync(FolderIdType folderId, FileIdType fileId, bool thumbNail = false)
        {
            var thumbNails = thumbNail ? "thumbnails/" : "";
            var fileName = $"{_prefix}/{folderId}/{thumbNails}{fileId}";
            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = _bucketName,
                Key = fileName
            };
            var deleteResponse = await _client.DeleteObjectAsync(deleteRequest);
            return deleteResponse.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }

        protected async Task DeleteFolderAsync(string folder)
        {
            string continuationToken = null;

            do
            {
                var listObjectRequest = new ListObjectsV2Request
                {
                    BucketName = _bucketName,
                    Prefix = $"{_prefix}/{folder}",
                    ContinuationToken = continuationToken
                };

                var logResponse = await _client.ListObjectsV2Async(listObjectRequest);
                if (logResponse.S3Objects.Any())
                {
                    var response = _client.DeleteObjectsAsync(new DeleteObjectsRequest
                    {
                        BucketName = _bucketName,
                        Objects = logResponse.S3Objects.Select(x => new KeyVersion { Key = x.Key }).ToList()
                    });
                }

                continuationToken = logResponse.ContinuationToken;
            } while (continuationToken != null);
        }

        public async Task DeleteAsync(FolderIdType folderId)
        {
            await DeleteFolderAsync($"{_prefix}/{folderId}/thumbnails");
            await DeleteFolderAsync($"{_prefix}/{folderId}");
        }

        protected Uri GetUri(string key)
        {
            if (!string.IsNullOrEmpty(_minioServerUrl))
                return new Uri($"{_minioServerUrl}/{_bucketName}/{key}");
            return new Uri($"https://{_bucketName}.s3-{_regionEndpoint.SystemName}.amazonaws.com/{key}");
        }

        protected async Task<Uri> GetAsync(FolderIdType folderId, FileIdType fileId)
        {
            var key = $"{ASSET_ASSETKEY}{folderId}:{fileId}";
            var cacheItem = CacheProviderManager.CacheProvider.Get<Uri>(key);
            if (cacheItem != null && cacheItem.Item != null)
                return cacheItem.Item;

            var getObjectMetadataRequest = new GetObjectMetadataRequest
            {
                BucketName = _bucketName,
                Key = $"{_prefix}/{folderId}/{fileId}"
            };
            var blob = await _client.GetObjectMetadataAsync(getObjectMetadataRequest);

            if (blob.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                var uri = GetUri(getObjectMetadataRequest.Key);
                CacheProviderManager.CacheProvider.Set(key, uri, new Arragro.Core.Common.CacheProvider.CacheSettings(new TimeSpan(0, 30, 0), true));
                return uri;
            }

            return null;
        }

        protected async Task<Uri> UploadAsync(FolderIdType folderId, FileIdType fileId, byte[] data, string mimeType)
        {
            using (var stream = new MemoryStream(data))
            {
                var putRequest = new PutObjectRequest
                {
                    BucketName = _bucketName,
                    Key = $"{_prefix}/{folderId}/{fileId}",
                    InputStream = stream,
                    ContentType = mimeType
                };

                putRequest.Headers.ContentType = mimeType;
                putRequest.Headers.CacheControl = $"public, max-age={_cacheControlMaxAge}";

                var putResponse = await _client.PutObjectAsync(putRequest);
                if (putResponse.HttpStatusCode != System.Net.HttpStatusCode.OK)
                    throw new Exception($"Something went wrong uploading to '{putRequest.BucketName}/{putRequest.Key}'.");

                stream.Close();
                return GetUri(putRequest.Key);
            }
        }

        protected async Task<Uri> GetImageThumbnailAsync(FolderIdType folderId, FileIdType fileId)
        {
            var key = $"{THUMBNAIL_ASSETKEY}{folderId}:{fileId}";
            var cacheItem = CacheProviderManager.CacheProvider.Get<Uri>(key);
            if (cacheItem != null && cacheItem.Item != null)
                return cacheItem.Item;

            var getObjectMetadataRequest = new GetObjectMetadataRequest
            {
                BucketName = _bucketName,
                Key = $"{_prefix}/{folderId}/thumbnails/{fileId}"
            };
            var blob = await _client.GetObjectMetadataAsync(getObjectMetadataRequest);

            if (blob.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                var uri = GetUri(getObjectMetadataRequest.Key);
                CacheProviderManager.CacheProvider.Set(key, uri, new Arragro.Core.Common.CacheProvider.CacheSettings(new TimeSpan(0, 30, 0), true));
                return uri;
            }

            return null;
        }

        public async Task ResetCacheControlAsync()
        {
            var listRequest = new ListObjectsV2Request
            {
                BucketName = _bucketName,
                Prefix = _prefix
            };

            do
            {
                var listResponse = await _client.ListObjectsV2Async(listRequest);

                foreach (var blob in listResponse.S3Objects)
                {
                    await this.ResetCloudBlobCacheControlAsync(blob, _cacheControlMaxAge);
                }
                listRequest.ContinuationToken = listResponse.ContinuationToken;
            }
            while (listRequest.ContinuationToken != null);
        }

        protected async Task ResetCloudBlobCacheControlAsync(S3Object s3Object, int cacheControlMaxAge)
        {
            var copyRequest = new CopyObjectRequest
            {
                SourceBucket = _bucketName,
                SourceKey = s3Object.Key,
                DestinationBucket = _bucketName,
                DestinationKey = s3Object.Key,
                MetadataDirective = S3MetadataDirective.REPLACE
            };
            copyRequest.Headers.CacheControl = $"public, max-age={cacheControlMaxAge}";
            await _client.CopyObjectAsync(copyRequest);
        }

        protected async Task<Uri> UploadThumbnailAsync(FolderIdType folderId, FileIdType fileId, byte[] data, string mimeType)
        {
            using (var stream = new MemoryStream(data))
            {
                var putRequest = new PutObjectRequest
                {
                    BucketName = _bucketName,
                    Key = $"{_prefix}/{folderId}/thumbnails/{fileId}",
                    InputStream = stream,
                    ContentType = mimeType
                };

                putRequest.Headers.ContentType = mimeType;
                putRequest.Headers.CacheControl = $"public, max-age={_cacheControlMaxAge}";

                var putResponse = await _client.PutObjectAsync(putRequest);
                if (putResponse.HttpStatusCode != System.Net.HttpStatusCode.OK)
                    throw new Exception($"Something went wrong uploading to '{putRequest.BucketName}/{putRequest.Key}'.");

                stream.Close();
                return GetUri(putRequest.Key);
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
            var fileName = $"{_prefix}/{folderId}/{fileId}";
            var newFileName = $"{_prefix}/{folderId}/{newFileId}";

            var newGetRequest = new GetObjectMetadataRequest
            {
                BucketName = _bucketName,
                Key = newFileName
            };

            try
            {
                var newGetResponse = await _client.GetObjectMetadataAsync(newGetRequest);
                var newUri = GetUri(fileName);
                throw new Exception($"The blob you want to create already exists - {newUri}!");
            }
            catch (AmazonS3Exception ex)
            {
                if (ex.ErrorCode == "NotFound")
                {
                    var oldGetRequest = new GetObjectRequest
                    {
                        BucketName = _bucketName,
                        Key = fileName
                    };

                    var oldRequest = await _client.GetObjectAsync(oldGetRequest);
                    var oldUri = GetUri(fileName);

                    if (oldRequest.HttpStatusCode == System.Net.HttpStatusCode.OK)
                    {
                        byte[] bytes;

                        using (var ms = new MemoryStream())
                        {
                            await oldRequest.ResponseStream.CopyToAsync(ms);
                            ms.Position = 0;
                            bytes = ms.ToArray();
                        }

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
                        var uri = await UploadAsync(folderId, newFileId, bytes, oldRequest.Headers.ContentType);
                        var thumbNailImageResult = await _imageService.ResizeAndProcessImageAsync(bytes, 250, 60, true);
                        Uri thumbnailUri = null;
                        if (thumbNailImageResult.IsImage)
                        {
                            thumbnailUri = await UploadAsync(folderId, newFileId, thumbNailImageResult.Bytes, oldRequest.Headers.ContentType, true);
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
                        throw new Exception($"The blob you want to copy doesn't exists - {oldUri}!");
                    }
                }
                else
                    throw;
            }
        }

        public async Task<Uri> UploadAsync(FolderIdType folderId, FileIdType fileId, byte[] data, string mimeType, bool thumbnail = false)
        {
            if (thumbnail)
                return await UploadThumbnailAsync(folderId, fileId, data, mimeType);
            return await UploadAsync(folderId, fileId, data, mimeType);
        }

        private async Task CopyObjectAsync(GetObjectMetadataRequest oldGetRequest, GetObjectMetadataRequest newGetRequest, string oldFileName)
        {
            var copyRequest = new CopyObjectRequest
            {
                SourceBucket = _bucketName,
                SourceKey = oldGetRequest.Key,
                DestinationBucket = _bucketName,
                DestinationKey = newGetRequest.Key
            };
            await _client.CopyObjectAsync(copyRequest);

            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = _bucketName,
                Key = oldFileName
            };

            await _client.DeleteObjectAsync(deleteRequest);
        }

        public async Task<Uri> RenameAsync(FolderIdType folderId, FileIdType fileId, FileIdType newFileId, bool thumbnail = false)
        {
            var fileName = thumbnail ? $"{_prefix}/{folderId}/thumbnails/{fileId}" : $"{_prefix}/{folderId}/{fileId}";
            var newFileName = thumbnail ? $"{_prefix}/{folderId}/thumbnails/{newFileId}" : $"{_prefix}/{folderId}/{newFileId}";

            var oldGetRequest = new GetObjectMetadataRequest
            {
                BucketName = _bucketName,
                Key = fileName
            };

            var oldGetResponse = await _client.GetObjectMetadataAsync(oldGetRequest);

            if (oldGetResponse.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                var newGetRequest = new GetObjectMetadataRequest
                {
                    BucketName = _bucketName,
                    Key = newFileName
                };

                try
                {
                    await _client.GetObjectMetadataAsync(newGetRequest);

                    var deleteRequest = new DeleteObjectRequest
                    {
                        BucketName = _bucketName,
                        Key = fileName
                    };

                    await _client.DeleteObjectAsync(deleteRequest);
                    await CopyObjectAsync(oldGetRequest, newGetRequest, fileName);
                }
                catch (AmazonS3Exception ex)
                {
                    if (ex.ErrorCode == "NotFound")
                    {
                        await CopyObjectAsync(oldGetRequest, newGetRequest, fileName);
                    }
                    else
                        throw;
                }
            }
            return GetUri(newFileName);
        }
    }
}
