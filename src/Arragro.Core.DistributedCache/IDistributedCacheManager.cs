using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Arragro.Core.DistributedCache
{
    public interface IDistributedCacheManager
    {
        T Get<T>(string key, Serializer serializer = Serializer.ProtoBuf);
        T Get<T>(string key, Func<T> func, Serializer serializer = Serializer.ProtoBuf);
        T Get<T>(string key, Func<T> func, DistributedCacheEntryOptions options, Serializer serializer = Serializer.ProtoBuf);
        Task<T> GetAsync<T>(string key, Serializer serializer = Serializer.ProtoBuf, CancellationToken token = default);
        Task<T> GetAsync<T>(string key, Func<T> func, Serializer serializer = Serializer.ProtoBuf, CancellationToken token = default);
        Task<T> GetAsync<T>(string key, Func<T> func, DistributedCacheEntryOptions options, Serializer serializer = Serializer.ProtoBuf, CancellationToken token = default);
        Task<T> GetAsync<T>(string key, Func<Task<T>> func, Serializer serializer = Serializer.ProtoBuf, CancellationToken token = default);
        Task<T> GetAsync<T>(string key, Func<Task<T>> func, DistributedCacheEntryOptions options, Serializer serializer = Serializer.ProtoBuf, CancellationToken token = default);
        void Remove(string key);
        Task RemoveAsync(string key, CancellationToken token = default);
        void Set<T>(string key, T value, Serializer serializer = Serializer.ProtoBuf);
        void Set<T>(string key, T value, DistributedCacheEntryOptions options, Serializer serializer = Serializer.ProtoBuf);
        Task SetAsync<T>(string key, T value, Serializer serializer = Serializer.ProtoBuf, CancellationToken token = default);
        Task SetAsync<T>(string key, T value, DistributedCacheEntryOptions options, Serializer serializer = Serializer.ProtoBuf, CancellationToken token = default);
    }
}