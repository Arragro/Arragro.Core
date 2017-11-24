using Arragro.Core.Common.Logging;
using System;

namespace Arragro.Core.Common.Exceptions
{
    public class SqlConnectivityException : Exception
    {
        public SqlConnectivityException(Exception ex, ILog log) : base("Sql connectivity issue", ex)
        {
            log.Error(ex.Message);
        }
    }
}
