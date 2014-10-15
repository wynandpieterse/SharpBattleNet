using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpBattleNet.Runtime.Utilities.Logging
{
    /// <summary>
    /// Provides a mechanism to create instances of <see cref="ILog" /> objects.
    /// </summary>
    public static class LogProvider
    {
        private static ILogProvider _currentLogProvider;

        /// <summary>
        /// Gets a logger for the specified type.
        /// </summary>
        /// <typeparam name="T">The type whose name will be used for the logger.</typeparam>
        /// <returns>An instance of <see cref="ILog"/></returns>
        public static ILog For<T>()
        {
            return GetLogger(typeof(T));
        }

        /// <summary>
        /// Gets a logger for the current class.
        /// </summary>
        /// <returns>An instance of <see cref="ILog"/></returns>
        public static ILog GetCurrentClassLogger()
        {
            var stackFrame = new StackFrame(1, false);
            return GetLogger(stackFrame.GetMethod().DeclaringType);
        }

        /// <summary>
        /// Gets a logger for the specified type.
        /// </summary>
        /// <param name="type">The type whose name will be used for the logger.</param>
        /// <returns>An instance of <see cref="ILog"/></returns>
        public static ILog GetLogger(Type type)
        {
            return GetLogger(type.FullName);
        }

        /// <summary>
        /// Gets a logger with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>An instance of <see cref="ILog"/></returns>
        public static ILog GetLogger(string name)
        {
            ILogProvider logProvider = _currentLogProvider ?? ResolveLogProvider();
            return logProvider == null ? new NoOpLogger() : (ILog)new LoggerExecutionWrapper(logProvider.GetLogger(name));
        }

        /// <summary>
        /// Sets the current log provider.
        /// </summary>
        /// <param name="logProvider">The log provider.</param>
        public static void SetCurrentLogProvider(ILogProvider logProvider)
        {
            _currentLogProvider = logProvider;
        }

        private static ILogProvider ResolveLogProvider()
        {
            try
            {
                if (NLogLogProvider.IsLoggerAvailable())
                {
                    return new NLogLogProvider();
                }
                if (Log4NetLogProvider.IsLoggerAvailable())
                {
                    return new Log4NetLogProvider();
                }
                if (EntLibLogProvider.IsLoggerAvailable())
                {
                    return new EntLibLogProvider();
                }
                return SerilogLogProvider.IsLoggerAvailable() ? new SerilogLogProvider() : null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Exception occured resolving a log proivder. Logging for this assembly {0} is disabled. {1}",
                    typeof(LogProvider).Assembly.FullName,
                    ex);
                return null;
            }
        }

        public class NoOpLogger : ILog
        {
            public bool Log(Level logLevel, Func<string> messageFunc)
            {
                return false;
            }

            public void Log<TException>(Level logLevel, Func<string> messageFunc, TException exception)
                where TException : Exception
            { }
        }
    }
}
