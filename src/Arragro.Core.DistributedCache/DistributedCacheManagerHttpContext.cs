using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Arragro.Core.DistributedCache
{
    public class DistributedCacheManagerHttpContext : DistributedCacheManager, IDistributedCacheManager
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DistributedCacheManagerHttpContext(
            IDistributedCache distributedCache, 
            DistributedCacheEntryOptions distributedCacheEntryOptions,
            IHttpContextAccessor httpContextAccessor,
            IDistributedCacheKeyPrefix distributedCacheKeyPrefix = null) 
            : base(distributedCache, distributedCacheEntryOptions, distributedCacheKeyPrefix)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private bool TestHttpContext()
        {
            return _httpContextAccessor != null && _httpContextAccessor.HttpContext != null;
        }

        private void SetHttpContextItem<T>(string key, T value)
        {
            if (TestHttpContext())
            {
                key = PrefixKey(key);
                if (_httpContextAccessor.HttpContext.Items.ContainsKey(key))
                {
                    _httpContextAccessor.HttpContext.Items[key] = value;
                }
                else
                {
                    _httpContextAccessor.HttpContext.Items.Add(PrefixKey(key), value);
                }
            }
        }

        private T GetHttpContextItem<T>(string key)
        {
            if (TestHttpContext())
            {
                var data = _httpContextAccessor.HttpContext.Items[PrefixKey(key)];
                if (data != null)
                    return (T)data;
            }
            return default;
        }

        private void RemoveHttpContextItem(string key)
        {
            if (TestHttpContext())
                _httpContextAccessor.HttpContext.Items.Remove(PrefixKey(key));
        }
        public override void Remove(string key)
        {
            base.Remove(key);
            RemoveHttpContextItem(PrefixKey(key));
        }

        public override async Task RemoveAsync(string key, CancellationToken token = default)
        {
            await base.RemoveAsync(key, token);
            RemoveHttpContextItem(PrefixKey(key));
        }

        public override void Set<T>(string key, T value, DistributedCacheEntryOptions options, Serializer serializer = Serializer.ProtoBuf)
        {
            base.Set(key, value, options, serializer);
            SetHttpContextItem(PrefixKey(key), value);
        }

        public override async Task SetAsync<T>(string key, T value, DistributedCacheEntryOptions options, Serializer serializer = Serializer.ProtoBuf, CancellationToken token = default)
        {
            await base.SetAsync(key, value, options, serializer, token);
            SetHttpContextItem(PrefixKey(key), value);
        }

        public override void Set<T>(string key, T value, Serializer serializer = Serializer.ProtoBuf)
        {
            base.Set(key, value, serializer);
            SetHttpContextItem(PrefixKey(key), value);
        }

        public override async Task SetAsync<T>(string key, T value, Serializer serializer = Serializer.ProtoBuf, CancellationToken token = default)
        {
            await base.SetAsync(key, value, serializer, token);
            SetHttpContextItem(PrefixKey(key), value);
        }

        public override T Get<T>(string key, Serializer serializer = Serializer.ProtoBuf)
        {
            var httpContextData = GetHttpContextItem<T>(key);
            if (httpContextData != null)
                return httpContextData;

            return ProcessByteArray<T>(_distributedCache.Get(PrefixKey(key)), serializer);
        }

        public override async Task<T> GetAsync<T>(string key, Serializer serializer = Serializer.ProtoBuf, CancellationToken token = default)
        {
            var httpContextData = GetHttpContextItem<T>(key);
            if (httpContextData != null)
                return httpContextData;

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

        public override T Get<T>(string key, Func<T> func, DistributedCacheEntryOptions options, Serializer serializer = Serializer.ProtoBuf)
        {
            var httpContextData = GetHttpContextItem<T>(key);
            if (httpContextData != null)
                return httpContextData;

            return base.Get(key, func, options, serializer);
        }

        public override async Task<T> GetAsync<T>(string key, Func<T> func, DistributedCacheEntryOptions options, Serializer serializer = Serializer.ProtoBuf, CancellationToken token = default)
        {
            var httpContextData = GetHttpContextItem<T>(key);
            if (httpContextData != null)
                return httpContextData;

            return await base.GetAsync(key, func, options, serializer, token);
        }

        public override async Task<T> GetAsync<T>(string key, Func<Task<T>> func, DistributedCacheEntryOptions options, Serializer serializer = Serializer.ProtoBuf, CancellationToken token = default)
        {
            var httpContextData = GetHttpContextItem<T>(key);
            if (httpContextData != null)
                return httpContextData;

            return await base.GetAsync(key, func, options, serializer, token);
        }
    }
}
