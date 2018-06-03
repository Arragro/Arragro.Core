using Arragro.Core.Common.CacheProvider;
using System;
using System.Threading;
using Xunit;

namespace Arragro.Core.Common.Tests.CacheProvider.UnitTests
{
    public class MemoryCacheUseCase
    {
        [Fact]
        public void TestCacheProviderDefaultsToMemoryCacheProvider()
        {
            Assert.Equal(typeof(MemoryCacheProvider), CacheProviderManager.CacheProvider.GetType());
        }

        [Fact]
        public void TestMemoryCacheProvider()
        {
            CacheProviderManager.CacheProvider = MemoryCacheProvider.GetInstance();
            var cacheProvider = CacheProviderManager.CacheProvider;
            var cacheSettings = new CacheSettings(new TimeSpan(0, 0, 0, 0, 20), true);

            cacheProvider.Set("Test", "Hello", cacheSettings);

            var data = cacheProvider.Get<string>("Test");
            Assert.Equal("Hello", data.Item);

            Thread.Sleep(20);

            data = cacheProvider.Get<string>("Test");
            Assert.Null(data);
        }

        [Fact]
        public void TestCacheUseCaseWithMemoryCache()
        {
            var cacheSettings = new CacheSettings(new TimeSpan(0, 0, 0, 0, 10), true);
            var data = Cache.Get<string>("Hello", () => "Hello", cacheSettings);
            Assert.Equal("Hello", data);

            Thread.Sleep(11);
            Assert.Null(Cache.Get<string>("Hello"));
        }
    }
}