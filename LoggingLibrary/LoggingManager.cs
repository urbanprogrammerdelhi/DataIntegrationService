using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoggingLibrary
{
    public static class LoggingManager
    {
        public static void LogException(Type type, Exception exception, string message = null)
        {
            LogManager
                .GetLogger(type.FullName)
                .Error(exception, $"Inner Exception : {exception.InnerException?.StackTrace}. Message : {message}");
        }

        public static void LogMessage(Type type, string message)
        {
            LogManager
                .GetLogger(type.FullName)
                .Info(message);
        }

        public static void LogFatal(Type type, Exception exception, string message)
        {
            LogManager
                .GetLogger(type.FullName)
                .Fatal(exception, message);
        }
    }
}
