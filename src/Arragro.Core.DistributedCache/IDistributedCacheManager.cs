using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Arragro.Core.DistributedCache
{
    public interface IDistributedCacheManager
    {
        T Get<T>(string key);
        T Get<T>(string key, Func<T> func);
        T Get<T>(string key, Func<T> func, DistributedCacheEntryOptions options);
        Task<T> GetAsync<T>(string key, CancellationToken token = default);
        Task<T> GetAsync<T>(string key, Func<T> func, CancellationToken token = default);
        Task<T> GetAsync<T>(string key, Func<T> func, DistributedCacheEntryOptions options, CancellationToken token = default);
        Task<T> GetAsync<T>(string key, Func<Task<T>> func, CancellationToken token = default);
        Task<T> GetAsync<T>(string key, Func<Task<T>> func, DistributedCacheEntryOptions options, CancellationToken token = default);
        void Remove(string key);
        Task RemoveAsync(string key, CancellationToken token = default);
        void Set<T>(string key, T value);
        void Set<T>(string key, T value, DistributedCacheEntryOptions options);
        Task SetAsync<T>(string key, T value, CancellationToken token = default);
        Task SetAsync<T>(string key, T value, DistributedCacheEntryOptions options, CancellationToken token = default);
    }
}