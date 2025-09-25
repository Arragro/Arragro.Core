using System;
using System.Collections.Generic;
using System.Text;

namespace Arragro.Core.Common.Models
{
    public enum CacheType
    {
        InMemory,
        Backplane,
        Full
    }

    public class CacheConfigurationSettings
    {
        public CacheType CacheType { get; set; }
        public string BackplaneRedisConnection { get; set; }
        public string CacheRedisConnection { get; set; }

        public void Validate()
        {
            if (CacheType == CacheType.Backplane && string.IsNullOrEmpty(BackplaneRedisConnection))
                throw new ArgumentException("If using a backplane, you must supply a redis connection string.");

            if (CacheType == CacheType.Full && string.IsNullOrEmpty(BackplaneRedisConnection))
                throw new ArgumentException("If using a backplane, you must supply a redis connection string.");

            if (CacheType == CacheType.Full && string.IsNullOrEmpty(CacheRedisConnection))
                throw new ArgumentException("If using a cache, you must supply a redis connection string.");
        }
    }
}
