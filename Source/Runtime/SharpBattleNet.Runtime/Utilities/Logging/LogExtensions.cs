using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpBattleNet.Runtime.Utilities.Logging
{
    public static class LogExtensions
    {
        public static bool IsDebugEnabled(this ILog logger)
        {
            GuardAgainstNullLogger(logger);
            return logger.Log(Level.Debug, null);
        }

        public static bool IsErrorEnabled(this ILog logger)
        {
            GuardAgainstNullLogger(logger);
            return logger.Log(Level.Error, null);
        }

        public static bool IsFatalEnabled(this ILog logger)
        {
            GuardAgainstNullLogger(logger);
            return logger.Log(Level.Fatal, null);
        }

        public static bool IsInfoEnabled(this ILog logger)
        {
            GuardAgainstNullLogger(logger);
            return logger.Log(Level.Info, null);
        }

        public static bool IsTraceEnabled(this ILog logger)
        {
            GuardAgainstNullLogger(logger);
            return logger.Log(Level.Trace, null);
        }

        public static bool IsWarnEnabled(this ILog logger)
        {
            GuardAgainstNullLogger(logger);
            return logger.Log(Level.Warn, null);
        }

        public static void Debug(this ILog logger, Func<string> messageFunc)
        {
            GuardAgainstNullLogger(logger);
            logger.Log(Level.Debug, messageFunc);
        }

        public static void Debug(this ILog logger, string message)
        {
            GuardAgainstNullLogger(logger);
            logger.Log(Level.Debug, () => message);
        }

        public static void DebugFormat(this ILog logger, string message, params object[] args)
        {
            GuardAgainstNullLogger(logger);
            logger.Log(Level.Debug, () => string.Format(CultureInfo.InvariantCulture, message, args));
        }

        public static void Error(this ILog logger, string message)
        {
            GuardAgainstNullLogger(logger);
            logger.Log(Level.Error, () => message);
        }

        public static void ErrorFormat(this ILog logger, string message, params object[] args)
        {
            GuardAgainstNullLogger(logger);
            logger.Log(Level.Error, () => string.Format(CultureInfo.InvariantCulture, message, args));
        }

        public static void ErrorException(this ILog logger, string message, Exception exception)
        {
            GuardAgainstNullLogger(logger);
            logger.Log(Level.Error, () => message, exception);
        }

        public static void Info(this ILog logger, Func<string> messageFunc)
        {
            GuardAgainstNullLogger(logger);
            logger.Log(Level.Info, messageFunc);
        }

        public static void Info(this ILog logger, string message)
        {
            GuardAgainstNullLogger(logger);
            logger.Log(Level.Info, () => message);
        }

        public static void InfoFormat(this ILog logger, string message, params object[] args)
        {
            GuardAgainstNullLogger(logger);
            logger.Log(Level.Info, () => string.Format(CultureInfo.InvariantCulture, message, args));
        }

        public static void Warn(this ILog logger, Func<string> messageFunc)
        {
            GuardAgainstNullLogger(logger);
            logger.Log(Level.Warn, messageFunc);
        }

        public static void Warn(this ILog logger, string message)
        {
            GuardAgainstNullLogger(logger);
            logger.Log(Level.Warn, () => message);
        }

        public static void WarnFormat(this ILog logger, string message, params object[] args)
        {
            GuardAgainstNullLogger(logger);
            logger.Log(Level.Warn, () => string.Format(CultureInfo.InvariantCulture, message, args));
        }

        public static void WarnException(this ILog logger, string message, Exception ex)
        {
            GuardAgainstNullLogger(logger);
            logger.Log(Level.Warn, () => string.Format(CultureInfo.InvariantCulture, message), ex);
        }

        private static void GuardAgainstNullLogger(ILog logger)
        {
            if (logger == null)
            {
                throw new ArgumentException("logger is null", "logger");
            }
        }
    }
}
