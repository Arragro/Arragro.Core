using Microsoft.Extensions.Caching.Distributed;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Arragro.Core.DistributedCache
{
    public class DistributedCacheManager
    {
        private readonly IDistributedCache _distributedCache;
        private readonly DistributedCacheEntryOptions _distributedCacheEntryOptions;
        private readonly IDistributedCacheKeyPrefix _distributedCacheKeyPrefix;

        public DistributedCacheManager(
            IDistributedCache distributedCache,
            DistributedCacheEntryOptions distributedCacheEntryOptions,
            IDistributedCacheKeyPrefix distributedCacheKeyPrefix = null)
        {
            _distributedCache = distributedCache;
            _distributedCacheEntryOptions = distributedCacheEntryOptions;
            if (distributedCacheKeyPrefix == null)
                _distributedCacheKeyPrefix = new DistributedCacheKeyPrefix();
            else
                _distributedCacheKeyPrefix = distributedCacheKeyPrefix;
        }

        private T ProcessByteArray<T>(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
                return default(T);
            T output;
            using (var ms = new MemoryStream(bytes))
            {
                try
                {
                    output = ProtoBuf.Serializer.Deserialize<T>(ms);
                }
                catch (Exception ex)
                {
                    var x = ex;
                    throw;
                }
            }
            return output;
        }

        private string PrefixKey(string key)
        {
            var prefix = _distributedCacheKeyPrefix.GeneratePrefix();
            if (!string.IsNullOrEmpty(prefix))
                return $"{prefix}:{key}";
            return key;
        }

        public T Get<T>(string key)
        {
            return ProcessByteArray<T>(_distributedCache.Get(PrefixKey(key)));
        }

        public async Task<T> GetAsync<T>(string key, CancellationToken token = default(CancellationToken))
        {
            try
            {
                var bytes = await _distributedCache.GetAsync(PrefixKey(key), token);
                return ProcessByteArray<T>(bytes);
            }
            catch (Exception ex)
            {
                var x = ex;
                throw;
            }
        }

        private byte[] ToProtoBufByteArray<T>(T value)
        {
            byte[] output;

            using (var ms = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(ms, value);
                output = ms.ToArray();
            }
            return output;
        }

        public void Set<T>(string key, T value, DistributedCacheEntryOptions options)
        {
            _distributedCache.Set(PrefixKey(key), ToProtoBufByteArray<T>(value), options);
        }

        public async Task SetAsync<T>(string key, T value, DistributedCacheEntryOptions options, CancellationToken token = default(CancellationToken))
        {
            await _distributedCache.SetAsync(PrefixKey(key), ToProtoBufByteArray<T>(value), options, token);
        }

        public void Set<T>(string key, T value)
        {
            _distributedCache.Set(PrefixKey(key), ToProtoBufByteArray<T>(value), _distributedCacheEntryOptions);
        }

        public async Task SetAsync<T>(string key, T value, CancellationToken token = default(CancellationToken))
        {
            await _distributedCache.SetAsync(PrefixKey(key), ToProtoBufByteArray<T>(value), _distributedCacheEntryOptions);
        }

        public void Remove(string key)
        {
            _distributedCache.Remove(PrefixKey(key));
        }

        public async Task RemoveAsync(string key, CancellationToken token = default(CancellationToken))
        {
            await _distributedCache.RemoveAsync(PrefixKey(key), token);
        }

        public T Get<T>(string key, Func<T> func)
        {
            return Get(PrefixKey(key), func, _distributedCacheEntryOptions);
        }

        public T Get<T>(string key, Func<T> func, DistributedCacheEntryOptions options)
        {
            var value = Get<T>(PrefixKey(key));
            if (value != null)
                return value;
            value = func();
            Set<T>(PrefixKey(key), value, options);
            return value;
        }

        public async Task<T> GetAsync<T>(string key, Func<Task<T>> func, CancellationToken token = default(CancellationToken))
        {
            return await GetAsync(PrefixKey(key), func, _distributedCacheEntryOptions, token);
        }

        public async Task<T> GetAsync<T>(string key, Func<T> func, CancellationToken token = default(CancellationToken))
        {
            return await GetAsync(PrefixKey(key), func, _distributedCacheEntryOptions, token);
        }

        public async Task<T> GetAsync<T>(string key, Func<T> func, DistributedCacheEntryOptions options, CancellationToken token = default(CancellationToken))
        {
            var value = await GetAsync<T>(PrefixKey(key), token);
            if (value != null)
                return value;
            value = func();
            await SetAsync<T>(PrefixKey(key), value, options, token);
            return value;
        }

        public async Task<T> GetAsync<T>(string key, Func<Task<T>> func, DistributedCacheEntryOptions options, CancellationToken token = default(CancellationToken))
        {
            var value = await GetAsync<T>(PrefixKey(key), token);
            if (value != null)
                return value;
            value = await func();
            await SetAsync<T>(PrefixKey(key), value, options, token);
            return value;
        }
    }
}
