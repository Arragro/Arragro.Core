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
        private readonly string _typeName;

        public DistributedCacheManager(
            IDistributedCache distributedCache,
            DistributedCacheEntryOptions distributedCacheEntryOptions)
        {
            _distributedCache = distributedCache;
            _distributedCacheEntryOptions = distributedCacheEntryOptions;
            _typeName = _distributedCache.GetType().ToString();
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

        public T Get<T>(string key)
        {
            return ProcessByteArray<T>(_distributedCache.Get(key));
        }

        public async Task<T> GetAsync<T>(string key, CancellationToken token = default(CancellationToken))
        {
            try
            {
                var bytes = await _distributedCache.GetAsync(key, token);
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
            _distributedCache.Set(key, ToProtoBufByteArray<T>(value), options);
        }

        public async Task SetAsync<T>(string key, T value, DistributedCacheEntryOptions options, CancellationToken token = default(CancellationToken))
        {
            await _distributedCache.SetAsync(key, ToProtoBufByteArray<T>(value), options, token);
        }

        public void Set<T>(string key, T value)
        {
            _distributedCache.Set(key, ToProtoBufByteArray<T>(value), _distributedCacheEntryOptions);
        }

        public async Task SetAsync<T>(string key, T value, CancellationToken token = default(CancellationToken))
        {
            await _distributedCache.SetAsync(key, ToProtoBufByteArray<T>(value), _distributedCacheEntryOptions);
        }

        public void Remove(string key)
        {
            _distributedCache.Remove(key);
        }

        public async Task RemoveAsync(string key, CancellationToken token = default(CancellationToken))
        {
            await _distributedCache.RemoveAsync(key, token);
        }

        public T Get<T>(string key, Func<T> func)
        {
            return Get(key, func, _distributedCacheEntryOptions);
        }

        public T Get<T>(string key, Func<T> func, DistributedCacheEntryOptions options)
        {
            var value = Get<T>(key);
            if (value != null)
                return value;
            value = func();
            Set<T>(key, value, options);
            return value;
        }

        public async Task<T> GetAsync<T>(string key, Func<Task<T>> func, CancellationToken token = default(CancellationToken))
        {
            return await GetAsync(key, func, _distributedCacheEntryOptions, token);
        }

        public async Task<T> GetAsync<T>(string key, Func<T> func, CancellationToken token = default(CancellationToken))
        {
            return await GetAsync(key, func, _distributedCacheEntryOptions, token);
        }

        public async Task<T> GetAsync<T>(string key, Func<T> func, DistributedCacheEntryOptions options, CancellationToken token = default(CancellationToken))
        {
            var value = await GetAsync<T>(key, token);
            if (value != null)
                return value;
            value = func();
            await SetAsync<T>(key, value, options, token);
            return value;
        }

        public async Task<T> GetAsync<T>(string key, Func<Task<T>> func, DistributedCacheEntryOptions options, CancellationToken token = default(CancellationToken))
        {
            var value = await GetAsync<T>(key, token);
            if (value != null)
                return value;
            value = await func();
            await SetAsync<T>(key, value, options, token);
            return value;
        }
    }
}
