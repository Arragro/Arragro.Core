using System;

namespace Arragro.Core.Common.Models
{
    public enum FusionCacheType
    {
        InMemory,
        Backplane,
        Distributed,
        BackplaneAndDistributed
    }

    public class FusionCacheConfigurationSettings
    {
        public FusionCacheType CacheType { get; set; }
        public string BackplaneRedisConnection { get; set; }
        public string DistributedCacheRedisConnection { get; set; }

        public void Validate()
        {
            if (CacheType == FusionCacheType.Backplane && string.IsNullOrEmpty(BackplaneRedisConnection))
                throw new ArgumentException("If using a backplane, you must supply a redis connection string.");

            if (CacheType == FusionCacheType.Distributed && string.IsNullOrEmpty(DistributedCacheRedisConnection))
                throw new ArgumentException("If using a cache, you must supply a redis connection string.");

            if (CacheType == FusionCacheType.BackplaneAndDistributed && string.IsNullOrEmpty(BackplaneRedisConnection))
                throw new ArgumentException("If using a backplane, you must supply a redis connection string.");

            if (CacheType == FusionCacheType.BackplaneAndDistributed && string.IsNullOrEmpty(DistributedCacheRedisConnection))
                throw new ArgumentException("If using a cache, you must supply a redis connection string.");
        }
    }
}
