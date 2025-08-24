using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Arragro.Core.DistributedCache
{
    public enum Serializer
    {
        ProtoBuf,
        Json
    }

    public class DistributedCacheManager : IDistributedCacheManager
    {
        protected readonly IDistributedCache _distributedCache;
        protected readonly DistributedCacheEntryOptions _distributedCacheEntryOptions;
        protected readonly IDistributedCacheKeyPrefix _distributedCacheKeyPrefix;

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

        protected T ProcessByteArray<T>(byte[] bytes, Serializer serializer = Serializer.ProtoBuf)
        {
            if (bytes == null || bytes.Length == 0)
                return default(T);
            T output;
            switch (serializer)
            {
                case Serializer.Json:
                    var json = Encoding.ASCII.GetString(bytes);
                    output = JsonConvert.DeserializeObject<T>(json);
                    break;
                default:
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
                    break;
            }
            return output;
        }

        protected string PrefixKey(string key)
        {
            var prefix = _distributedCacheKeyPrefix.GeneratePrefix();
            if (!string.IsNullOrEmpty(prefix))
                return $"{prefix}:{key}";
            return key;
        }

        public virtual T Get<T>(string key, Serializer serializer = Serializer.ProtoBuf)
        {
            return ProcessByteArray<T>(_distributedCache.Get(PrefixKey(key)), serializer);
        }

        public virtual async Task<T> GetAsync<T>(string key, Serializer serializer = Serializer.ProtoBuf, CancellationToken token = default)
        {
            try
            {
                var bytes = await _distributedCache.GetAsync(PrefixKey(key), token);
                return ProcessByteArray<T>(bytes, serializer);
            }
            catch (Exception ex)
            {
                var x = ex;
                throw;
            }
        }

        private byte[] ToByteArray<T>(T value, Serializer serializer = Serializer.ProtoBuf)
        {
            byte[] output;

            switch (serializer)
            {
                case Serializer.Json:
                    output = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(value));
                    break;
                default:
                    using (var ms = new MemoryStream())
                    {
                        ProtoBuf.Serializer.Serialize(ms, value);
                        output = ms.ToArray();
                    }
                    break;
            }

            return output;
        }

        public virtual void Set<T>(string key, T value, DistributedCacheEntryOptions options, Serializer serializer = Serializer.ProtoBuf)
        {
            _distributedCache.Set(PrefixKey(key), ToByteArray<T>(value, serializer), options);
        }

        public virtual async Task SetAsync<T>(string key, T value, DistributedCacheEntryOptions options, Serializer serializer = Serializer.ProtoBuf, CancellationToken token = default)
        {
            await _distributedCache.SetAsync(PrefixKey(key), ToByteArray<T>(value, serializer), options, token);
        }

        public virtual void Set<T>(string key, T value, Serializer serializer = Serializer.ProtoBuf)
        {
            _distributedCache.Set(PrefixKey(key), ToByteArray<T>(value, serializer), _distributedCacheEntryOptions);
        }

        public virtual async Task SetAsync<T>(string key, T value, Serializer serializer = Serializer.ProtoBuf, CancellationToken token = default)
        {
            await _distributedCache.SetAsync(PrefixKey(key), ToByteArray<T>(value, serializer), _distributedCacheEntryOptions);
        }

        public virtual void Remove(string key)
        {
            _distributedCache.Remove(PrefixKey(key));
        }

        public virtual async Task RemoveAsync(string key, CancellationToken token = default(CancellationToken))
        {
            await _distributedCache.RemoveAsync(PrefixKey(key), token);
        }

        public T Get<T>(string key, Func<T> func, Serializer serializer = Serializer.ProtoBuf)
        {
            return Get(PrefixKey(key), func, _distributedCacheEntryOptions, serializer);
        }

        public virtual T Get<T>(string key, Func<T> func, DistributedCacheEntryOptions options, Serializer serializer = Serializer.ProtoBuf)
        {
            var value = Get<T>(PrefixKey(key), serializer);
            if (value != null)
                return value;
            value = func();
            Set<T>(PrefixKey(key), value, options, serializer);
            return value;
        }

        public async Task<T> GetAsync<T>(string key, Func<Task<T>> func, Serializer serializer = Serializer.ProtoBuf, CancellationToken token = default)
        {
            return await GetAsync(PrefixKey(key), func, _distributedCacheEntryOptions, serializer, token);
        }

        public async Task<T> GetAsync<T>(string key, Func<T> func, Serializer serializer = Serializer.ProtoBuf, CancellationToken token = default)
        {
            return await GetAsync(PrefixKey(key), func, _distributedCacheEntryOptions, serializer, token);
        }

        public virtual async Task<T> GetAsync<T>(string key, Func<T> func, DistributedCacheEntryOptions options, Serializer serializer = Serializer.ProtoBuf, CancellationToken token = default)
        {
            var value = await GetAsync<T>(PrefixKey(key), serializer, token);
            if (value != null)
                return value;
            value = func();
            await SetAsync<T>(PrefixKey(key), value, options, serializer, token);
            return value;
        }

        public virtual async Task<T> GetAsync<T>(string key, Func<Task<T>> func, DistributedCacheEntryOptions options, Serializer serializer = Serializer.ProtoBuf, CancellationToken token = default(CancellationToken))
        {
            var value = await GetAsync<T>(PrefixKey(key), serializer, token);
            if (value != null)
                return value;
            value = await func();
            await SetAsync<T>(PrefixKey(key), value, options, serializer, token);
            return value;
        }
    }
}
