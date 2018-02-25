using System;

namespace Arragro.Core.Web.Http
{
    public class HttpCache
    {
        public string ETag { get; set; }
        public DateTime? LastModified { get; set; }

        public bool IsCached(string etag, DateTime lastModified)
        {
            if (etag == ETag)
                return true;

            if (LastModified.HasValue && LastModified.Value >= lastModified )
                return true;

            return false;            
        }
    }
}
