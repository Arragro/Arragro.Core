using System.Threading.Tasks;

namespace Arragro.Core.Fastly
{
    public interface IFastlyClient
    {
        Task<bool> PurgeKeysAsync(string[] keys, int? waitMilliseconds = null);
        Task<bool> PurgeAllAsync(string serviceId);
        Task<bool> PurgeAllAsync(int? waitMilliseconds = null);
    }
}