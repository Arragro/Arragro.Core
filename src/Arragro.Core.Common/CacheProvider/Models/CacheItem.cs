using System;

namespace Arragro.Core.Common.CacheProvider
{
    public class CacheItem : ICacheItem
    {
        protected DateTime? GetExpiration(TimeSpan? cacheDuration)
        {
            DateTime? expiration = DateTime.UtcNow;

            if (cacheDuration.HasValue)
                expiration = expiration.Value.Add(cacheDuration.Value);
            else
                expiration = null;

            return expiration;
        }

        public Guid Identifier { get; protected set; }
        public string Key { get; protected set; }
        public DateTime CreatedDate { get; protected set; }
        public CacheSettings CacheSettings { get; protected set; }
        public DateTime? Expiration { get; protected set; }
        public int ByteLength { get; protected set; }

        public CacheItem(
            string key,
            CacheSettings cacheSettings)
        {
            Identifier = Guid.NewGuid();
            Key = key;
            CreatedDate = DateTime.UtcNow;
            if (cacheSettings != null)
            {
                CacheSettings = cacheSettings;
                Expiration = GetExpiration(cacheSettings.CacheDuration);
            }
        }

        public void ResetExpiration()
        {
            var expiration = GetExpiration(CacheSettings.CacheDuration);
            Expiration = expiration;
        }
    }
    
    public class CacheItem<T> : CacheItem, ICacheItem<T>
    {
        public T Item { get; set; }

        public CacheItem(
            string key,
            T item,
            CacheSettings cacheSettings)
            : base(key, cacheSettings)
        {
            Item = item;
        }
    }
}