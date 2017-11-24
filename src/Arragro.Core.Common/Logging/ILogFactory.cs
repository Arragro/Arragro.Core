using System;

namespace Arragro.Core.Common.Logging
{
    /*
     * Used by the LogFactory to implement a Factory pattern to return
     * a logger which implements Arragro.Core.Common.Logging.ILog.
     */
    public interface ILogFactory
    {
        ILog GetLogger(Type type);
        ILog GetLogger(string typeName);
    }
}
