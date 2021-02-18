using Azure;
using Azure.Storage.Blobs;
using System.Net;
using System.Threading.Tasks;

namespace Arragro.Providers.AzureStorageProvider
{
    public static class TempWorkAroundExtentions
    {
        public static async Task<bool> ExistsWorkAroundAsync(this BlobClient blobClient)
        {
            try
            {
                return await blobClient.ExistsAsync();
            }
            catch (RequestFailedException ex)
            {
                if (ex.Status == (int)HttpStatusCode.NotFound)
                {
                    return false;
                }
                throw;
            }
        }
    }
}
