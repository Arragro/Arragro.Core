using System;

namespace Arragro.Core.Common.Logging
{
    /* 
     * Slimmed down version of the ILog that log4net provides in order to created a
     * common logging experience in applications.  Allows for the implementation of 
     * a System.Diagnostic.Debug implementation, or NLog, etc.
     */
    public interface ILog
    {
        bool IsDebugEnabled { get; }
        void Debug(object message, Exception exception = null);
        void DebugFormat(string format, params object[] args);
        void Error(object message, Exception exception = null);
        void ErrorFormat(string format, params object[] args);
        void Fatal(object message, Exception exception = null);
        void FatalFormat(string format, params object[] args);
        void Info(object message, Exception exception = null);
        void InfoFormat(string format, params object[] args);
        void Warn(object message, Exception exception = null);
        void WarnFormat(string format, params object[] args);
    }
}
