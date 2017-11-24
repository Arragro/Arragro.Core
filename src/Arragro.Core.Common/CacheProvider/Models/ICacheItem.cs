using System;
using System.Collections.Generic;

namespace Arragro.Core.Common.CacheProvider
{
    public interface ICacheItem
    {
        Guid Identifier { get; }
        string Key { get; }
        DateTime CreatedDate { get; }
        CacheSettings CacheSettings { get; }
        DateTime? Expiration { get; }
        int ByteLength { get; }

        void ResetExpiration();
    }

    public interface ICacheItem<T> : ICacheItem
    {
        T Item { get; set; }
    }

    public interface ICacheItemList<T> : ICacheItem
    {
        IEnumerable<T> Items { get; set; }
    }
}