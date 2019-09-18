using System;
using ProtoBuf;
using ProtoBuf.Meta;

namespace Arragro.Core.DistributedCache
{
    [ProtoContract]
    public class DateTimeOffsetSurrogate
    {
        private static bool _isConfigured = false;
        private static object _locker = new object();

        [ProtoMember(1)]
        public string DateTimeString { get; set; }

        public static implicit operator DateTimeOffsetSurrogate(DateTimeOffset value)
        {
            return new DateTimeOffsetSurrogate { DateTimeString = value.ToString("o") };
        }

        public static implicit operator DateTimeOffset(DateTimeOffsetSurrogate value)
        {
            return DateTimeOffset.Parse(value.DateTimeString);
        }

        public static void Configure()
        {
            if (!_isConfigured)
            {
                lock (_locker)
                {
                    if (!_isConfigured)
                    {
                        RuntimeTypeModel.Default.Add(typeof(DateTimeOffset), false).SetSurrogate(typeof(DateTimeOffsetSurrogate));
                        _isConfigured = true;
                    }
                }
            }
        }
    }
}
