using System.Threading.Tasks;

namespace Arragro.Core.Fastly
{
    public interface IFastlyClient
    {
        Task<bool> PurgeKeysAsync(string[] keys, int? waitMilliseconds = null, bool softPurge = true);
        Task<bool> PurgeAllAsync(string serviceId, bool softPurge = true);
        Task<bool> PurgeAllAsync(int? waitMilliseconds = null, bool softPurge = true);
    }
}