using System;

namespace Arragro.Core.Common.Logging
{
    /*
     * Implemented to allow for test logging to occur using System.Diagnostic.Deg=bug.
     */
    public class DebugLogger : ILog
    {
        const string DEBUG = "DEBUG: ";
        const string ERROR = "ERROR: ";
        const string FATAL = "FATAL: ";
        const string INFO = "INFO: ";
        const string WARN = "WARN: ";

        public bool IsDebugEnabled { get; set; }

        public DebugLogger(Type type) { }

        public DebugLogger(string type) { }

        private static void Logger(object message, Exception exception = null)
        {
            var output = message == null ? String.Empty : message.ToString();
            
            if (exception != null)
            {
                output = string.Format("{0}, Exception: {1}", output, exception.Message);
            }

            System.Diagnostics.Debug.WriteLine(output);
        }

        private static void LoggerFormat(object message, params object[] args)
        {
            var output = message == null ? String.Empty : message.ToString();
            System.Diagnostics.Debug.WriteLine(string.Format(output, args));
        }
        
        public void Debug(object message, Exception exception = null)
        {
            Logger(DEBUG + message, exception);
        }

        public void DebugFormat(string format, params object[] args)
        {
            LoggerFormat(DEBUG + format, args);
        }

        public void Error(object message, Exception exception = null)
        {
            Logger(ERROR + message, exception);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            LoggerFormat(DEBUG + format, args);
        }

        public void Fatal(object message, Exception exception = null)
        {
            Logger(FATAL + message, exception);
        }

        public void FatalFormat(string format, params object[] args)
        {
            LoggerFormat(FATAL + format, args);
        }

        public void Info(object message, Exception exception = null)
        {
            Logger(INFO + message, exception);
        }

        public void InfoFormat(string format, params object[] args)
        {
            LoggerFormat(INFO + format, args);
        }

        public void Warn(object message, Exception exception = null)
        {
            Logger(WARN + message, exception);
        }

        public void WarnFormat(string format, params object[] args)
        {
            LoggerFormat(WARN + format, args);
        }
    }
}
