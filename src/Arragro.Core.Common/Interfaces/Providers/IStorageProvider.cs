using Arragro.Core.Common.Models;
using System;
using System.Threading.Tasks;

namespace Arragro.Core.Common.Interfaces.Providers
{
    public interface IStorageProvider<FolderIdType, FileIdType> 
    {
        Task<bool> DeleteAsync(FolderIdType folderId, FileIdType fileId, bool thumbNail = false);
        Task DeleteAsync(FolderIdType folderId);
        Task<Uri> GetAsync(FolderIdType folderId, FileIdType fileId, bool thumbnail = false);
        Task<CreateAssetFromExistingResult> CreateAssetFromExistingAsync(FolderIdType folderId, FileIdType fileId, FileIdType newFileId);
        Task<CreateAssetFromExistingResult> CreateAssetFromExistingAndResizeAsync(FolderIdType folderId, FileIdType fileId, FileIdType newFileId, int width, int quality = 80, bool asProgressiveJpeg = false);
        Task<Uri> UploadAsync(FolderIdType folderId, FileIdType fileId, byte[] data, string mimeType, bool thumbnail = false);
        Task<Uri> RenameAsync(FolderIdType folderId, FileIdType fileId, FileIdType newFileId, bool thumbnail = false);
        Task ResetCacheControlAsync();
    }
}
