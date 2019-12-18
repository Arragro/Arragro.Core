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

        private void SetHttpContextItem<T>(string key, T value)
        {
            if (_httpContextAccessor != null)
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
            if (_httpContextAccessor != null)
            {
                var data = _httpContextAccessor.HttpContext.Items[PrefixKey(key)];
                if (data != null)
                    return (T)data;
            }
            return default;
        }

        private void RemoveHttpContextItem(string key)
        {
            if (_httpContextAccessor != null)
                _httpContextAccessor.HttpContext.Items.Remove(PrefixKey(key));
        }
        public override void Remove(string key)
        {
            base.Remove(key);
            RemoveHttpContextItem(PrefixKey(key));
        }

        public override async Task RemoveAsync(string key, CancellationToken token = default(CancellationToken))
        {
            await base.RemoveAsync(key, token);
            RemoveHttpContextItem(PrefixKey(key));
        }

        public override void Set<T>(string key, T value, DistributedCacheEntryOptions options)
        {
            base.Set(key, value, options);
            SetHttpContextItem(PrefixKey(key), value);
        }

        public override async Task SetAsync<T>(string key, T value, DistributedCacheEntryOptions options, CancellationToken token = default(CancellationToken))
        {
            await base.SetAsync(key, value, options, token);
            SetHttpContextItem(PrefixKey(key), value);
        }

        public override void Set<T>(string key, T value)
        {
            base.Set(key, value);
            SetHttpContextItem(PrefixKey(key), value);
        }

        public override async Task SetAsync<T>(string key, T value, CancellationToken token = default(CancellationToken))
        {
            await base.SetAsync(key, value, token);
            SetHttpContextItem(PrefixKey(key), value);
        }

        public override T Get<T>(string key)
        {
            var httpContextData = GetHttpContextItem<T>(key);
            if (httpContextData != null)
                return httpContextData;

            return ProcessByteArray<T>(_distributedCache.Get(PrefixKey(key)));
        }

        public override async Task<T> GetAsync<T>(string key, CancellationToken token = default(CancellationToken))
        {
            var httpContextData = GetHttpContextItem<T>(key);
            if (httpContextData != null)
                return httpContextData;

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

        public override T Get<T>(string key, Func<T> func, DistributedCacheEntryOptions options)
        {
            var httpContextData = GetHttpContextItem<T>(key);
            if (httpContextData != null)
                return httpContextData;

            return base.Get(key, func, options);
        }

        public override async Task<T> GetAsync<T>(string key, Func<T> func, DistributedCacheEntryOptions options, CancellationToken token = default(CancellationToken))
        {
            var httpContextData = GetHttpContextItem<T>(key);
            if (httpContextData != null)
                return httpContextData;

            return await base.GetAsync(key, func, options, token);
        }

        public override async Task<T> GetAsync<T>(string key, Func<Task<T>> func, DistributedCacheEntryOptions options, CancellationToken token = default(CancellationToken))
        {
            var httpContextData = GetHttpContextItem<T>(key);
            if (httpContextData != null)
                return httpContextData;

            return await base.GetAsync(key, func, options, token);
        }
    }
}
