using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace Arragro.Core.Common.CacheProvider
{
    /// <summary>
    /// Simple key/value cache provider for local in memory use in applications.
    /// </summary>
    public class MemoryCacheProvider : ICacheProvider
    {
        private static readonly Lazy<MemoryCacheProvider> LazyMemoryCacheProvider =
            new Lazy<MemoryCacheProvider>(() => new MemoryCacheProvider(), true);

        private readonly static ConcurrentDictionary<string, object> Dictionary = new ConcurrentDictionary<string, object>();

        private bool CheckAndClearExpiredData(ICacheItem cacheItem)
        {
            var now = DateTimeOffset.UtcNow;
            var output = false;
            if (cacheItem.Expiration < now)
            {
                Trace.Write(string.Format("Removing: {0} Over in milliseconds: {1}", cacheItem.Key, (now - cacheItem.Expiration).Value.TotalMilliseconds));
                output = RemoveFromCache(cacheItem.Key);
            }
            PurgeExpiredCacheItems(cacheItem.Key);
            return output;
        }

        private void PurgeExpiredCacheItems(string excludeKey)
        {
            var now = DateTimeOffset.UtcNow;
            var cacheItems = Dictionary.Values.Where(x => ((ICacheItem)x).Expiration < now).Select(x => ((ICacheItem)x));
            foreach(var cacheItem in cacheItems)
            {
                if (cacheItem.Key != excludeKey)
                {
                    Trace.Write(string.Format("Removing: {0} Over in milliseconds: {1}", cacheItem.Key, (now - cacheItem.Expiration).Value.TotalMilliseconds));
                    RemoveFromCache(cacheItem.Key);
                }
            }
        }

        private T GetCacheItem<T>(string key) where T : ICacheItem
        {
            object item;
            T cacheItemList = default(T);
            Dictionary.TryGetValue(key, out item);

            if (item != null)
            {
                try
                {
                    var typedItem = (T)item;
                    cacheItemList = typedItem;
                    if (CheckAndClearExpiredData(cacheItemList))
                        item = null;
                }
                catch (InvalidCastException)
                {
                    Dictionary.Remove(key, out item);
                    item = null;
                }

            }
            return item == null ? default(T) : cacheItemList;
        }

        public static MemoryCacheProvider GetInstance()
        {
            return LazyMemoryCacheProvider.Value;
        }

        public MasterKeys GetAllCacheItems()
        {
            var mks =
                Dictionary.Select(
                    d => new MasterKey { Key = d.Key, ByteLength = 0, Expiration = ((ICacheItem)d.Value).Expiration })
                          .ToList();
            return new MasterKeys { Keys = mks };
        }

        public ICacheItemList<T> GetList<T>(string key)
        {
            var cacheItemList = GetCacheItem<ICacheItemList<T>>(key);
            if (cacheItemList != default(ICacheItemList<T>))
                if (cacheItemList.CacheSettings.SlidingExpiration)
                    cacheItemList = SetList(key, cacheItemList.Items, new CacheSettings(cacheItemList.CacheSettings.CacheDuration, cacheItemList.CacheSettings.SlidingExpiration));
            return cacheItemList;
        }

        public ICacheItem<T> Get<T>(string key)
        {
            var cacheItem = GetCacheItem<ICacheItem<T>>(key);
            if (cacheItem != default(ICacheItem<T>))
                if (cacheItem.CacheSettings.SlidingExpiration)
                    cacheItem = Set(key, cacheItem.Item, new CacheSettings(cacheItem.CacheSettings.CacheDuration, cacheItem.CacheSettings.SlidingExpiration));
            return cacheItem;
        }

        public ICacheItemList<T> SetList<T>(string key, IEnumerable<T> data, CacheSettings cacheSettings)
        {
            return (ICacheItemList<T>)Dictionary.AddOrUpdate(
                key,
                new CacheItemList<T>(key, data, cacheSettings),
                (keyToFind, oldItem) => new CacheItemList<T>(key, data, cacheSettings));
        }

        public ICacheItem<T> Set<T>(string key, T data, CacheSettings cacheSettings)
        {
            return (ICacheItem<T>)Dictionary.AddOrUpdate(
                key,
                new CacheItem<T>(key, data, cacheSettings),
                (keyToFind, oldItem) => new CacheItem<T>(key, data, cacheSettings));
        }

        public void RemoveAll()
        {
            Dictionary.Clear();
        }

        public bool RemoveFromCache(string key, bool pattern = false)
        {
            object item;
            if (pattern)
            {
                var regex = new Regex(key, RegexOptions.IgnoreCase);
                var keys = Dictionary.Keys.Where(k => regex.Match(k).Success).ToList();
                keys.ForEach(k => Dictionary.TryRemove(k, out item));
                return true;
            }
            return Dictionary.TryRemove(key, out item);
        }
    }
}