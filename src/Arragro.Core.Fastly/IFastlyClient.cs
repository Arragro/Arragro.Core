using System.Threading.Tasks;

namespace Arragro.Core.Fastly
{
    public interface IFastlyClient
    {
        Task<bool> PurgeKeysAsync(string serviceId, string[] keys);
    }
}