﻿using System.Net.Http;
using System.Threading.Tasks;

namespace Arragro.Core.Fastly
{
    public class DummyFastlyClient : IFastlyClient
    {
        public DummyFastlyClient(
            HttpClient httpClient)
        {

        }

        public async Task<bool> PurgeAllAsync(string serviceId)
        {
            await Task.Yield();
            return true;
        }

        public async Task<bool> PurgeAllAsync()
        {
            await Task.Yield();
            return true;
        }

        public async Task<bool> PurgeKeysAsync(string[] keys)
        {
            await Task.Yield();
            return true;
        }
    }
}
