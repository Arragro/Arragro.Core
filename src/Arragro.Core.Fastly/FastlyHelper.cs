using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Arragro.Core.Fastly
{
    public class FastlyHelper
    {
        private readonly FastlyApiTokens _fastlyApiTokens;

        public FastlyHelper(
            FastlyApiTokens fastlyApiTokens)
        {
            _fastlyApiTokens = fastlyApiTokens;
        }
        
        public void AddFastlySurrogateKeyToHeader(HttpResponse response, IEnumerable<string> keys)
        {
            if (_fastlyApiTokens.Enabled)
            {
                var responseHeaders = response.GetTypedHeaders();
                responseHeaders.Append("Surrogate-Key", String.Join(" ", keys));
            }
        }
        
        public void AddFastlySurrogateCacheControlToHeader(HttpResponse response, long maxAge, long? staleWhileRevalidating = null, long? staleIfError = null)
        {
            if (_fastlyApiTokens.Enabled)
            {
                var sb = new StringBuilder($"max-age={maxAge}");
                if (staleWhileRevalidating.HasValue) sb.Append($", stale-while-revalidate={staleWhileRevalidating}");
                if (staleIfError.HasValue) sb.Append($", stale-if-error={staleWhileRevalidating}");
                
                
                var responseHeaders = response.GetTypedHeaders();
                responseHeaders.Append("Surrogate-Control", sb.ToString());
            }
        }
    }
}