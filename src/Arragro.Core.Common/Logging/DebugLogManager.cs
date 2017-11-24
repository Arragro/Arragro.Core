using System;

namespace Arragro.Core.Common.Logging
{
    public class DebugLogManager : ILogFactory
    {
        private readonly bool _debugEnabled;

        public DebugLogManager(bool debugEnabled = true)
        {
            _debugEnabled = debugEnabled;
        }

        public ILog GetLogger(Type type)
        {
            return new DebugLogger(type) { IsDebugEnabled = _debugEnabled };
        }

        public ILog GetLogger(string typeName)
        {
            return new DebugLogger(typeName) { IsDebugEnabled = _debugEnabled };
        }
    }
}
