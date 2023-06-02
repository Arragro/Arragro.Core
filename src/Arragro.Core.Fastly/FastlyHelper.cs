using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Arragro.Core.Fastly
{
    public class FastlyHelper
    {
        public FastlyHelper()
        {
        }
        
        public void AddFastlySurrogateKeyToHeader(HttpResponse response, IEnumerable<string> keys)
        {
            var responseHeaders = response.GetTypedHeaders();
            var all = new List<string> { "all" };
            all.AddRange(keys);
            responseHeaders.Append("Surrogate-Key", String.Join(" ", keys));
        }
        
        public void AddFastlySurrogateCacheControlToHeader(HttpResponse response, long maxAge, long? staleWhileRevalidating = null, long? staleIfError = null)
        {
            var sb = new StringBuilder($"max-age={maxAge}");
            if (staleWhileRevalidating.HasValue) sb.Append($", stale-while-revalidate={staleWhileRevalidating}");
            if (staleIfError.HasValue) sb.Append($", stale-if-error={staleIfError}");
                
                
            var responseHeaders = response.GetTypedHeaders();
            responseHeaders.Append("Surrogate-Control", sb.ToString());
        }
    }
}