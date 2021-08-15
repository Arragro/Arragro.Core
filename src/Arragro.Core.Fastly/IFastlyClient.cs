using System.Threading.Tasks;

namespace Arragro.Core.Fastly
{
    public interface IFastlyClient
    {
        Task PurgeKeysAsync(string[] keys);
    }
}