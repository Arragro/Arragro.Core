using System.Net.Http;
using System.Threading.Tasks;

namespace Arragro.Core.Fastly
{
    public class DummyFastlyClient : IFastlyClient
    {
        public DummyFastlyClient(
            HttpClient httpClient)
        {

        }

        public async Task PurgeKeysAsync(string[] keys)
        {
            await Task.Yield();
        }
    }
}
