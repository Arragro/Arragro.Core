using System;

namespace Arragro.Core.Common.Interfaces
{
    public interface IDictionaryKey
    {
        string DictionaryKey { get; set; }
        DateTimeOffset LastWriteDate { get; set; }
    }
}
