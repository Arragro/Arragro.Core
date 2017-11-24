using Arragro.Core.Common.Models;
using System;
using System.Threading.Tasks;

namespace Arragro.Core.Common.Interfaces.Providers
{
    public interface IStorageProvider<FolderIdType, FileIdType> 
    {
        Task<bool> Delete(FolderIdType folderId, FileIdType fileId, bool thumbNail = false);
        Task Delete(FolderIdType folderId);
        Task<Uri> Get(FolderIdType folderId, FileIdType fileId, bool thumbnail = false);
        Task<CreateImageFromImageResult> CreateImageFromExistingImage(FolderIdType folderId, FileIdType fileId, FileIdType newFileId, int quality, int width, bool asProgressive = true);
        Task<Uri> Upload(FolderIdType folderId, FileIdType fileId, byte[] data, string mimeType, bool thumbnail = false);
        Task<Uri> Rename(FolderIdType folderId, FileIdType fileId, FileIdType newFileId, bool thumbnail = false);
        Task ResetCacheControl();
    }
}
