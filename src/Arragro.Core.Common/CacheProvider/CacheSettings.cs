using System;

namespace Arragro.Core.Common.CacheProvider
{
    public class CacheSettings
    {
        public TimeSpan? CacheDuration { get; private set; }

        public bool SlidingExpiration { get; private set; }

        public CacheSettings()
        {
            CacheDuration = null;
            SlidingExpiration = false;
        }

        public CacheSettings(
            TimeSpan? cacheDuration,
            bool slidingExpiration = false)
        {
            CacheDuration = cacheDuration;
            SlidingExpiration = slidingExpiration;
        }
    }
}