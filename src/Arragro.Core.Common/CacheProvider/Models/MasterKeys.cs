using System;
using System.Collections.Generic;

namespace Arragro.Core.Common.CacheProvider
{
    public class MasterKey
    {
        public string Key { get; set; }
        public int ByteLength { get; set; }
        public DateTime? Expiration { get; set; }
    }

    public class MasterKeys
    {
        public MasterKeys()
        {
            Keys = new List<MasterKey>();
        }
        public IList<MasterKey> Keys { get; set; }
    }
}
