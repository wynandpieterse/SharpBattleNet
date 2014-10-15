using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpBattleNet.Runtime.Utilities.Logging.Providers
{
    public class Log4NetLogProvider : ILogProvider
    {
        private readonly Func<string, object> _getLoggerByNameDelegate;
        private static bool _providerIsAvailableOverride = true;

        public Log4NetLogProvider()
        {
            if (!IsLoggerAvailable())
            {
                throw new InvalidOperationException("log4net.LogManager not found");
            }
            _getLoggerByNameDelegate = GetGetLoggerMethodCall();
        }

        public static bool ProviderIsAvailableOverride
        {
            get { return _providerIsAvailableOverride; }
            set { _providerIsAvailableOverride = value; }
        }

        public ILog GetLogger(string name)
        {
            return new Log4NetLogger(_getLoggerByNameDelegate(name));
        }

        public static bool IsLoggerAvailable()
        {
            return ProviderIsAvailableOverride && GetLogManagerType() != null;
        }

        private static Type GetLogManagerType()
        {
            return Type.GetType("log4net.LogManager, log4net");
        }

        private static Func<string, object> GetGetLoggerMethodCall()
        {
            Type logManagerType = GetLogManagerType();
            MethodInfo method = logManagerType.GetMethod("GetLogger", new[] { typeof(string) });
            ParameterExpression nameParam = Expression.Parameter(typeof(string), "name");
            MethodCallExpression methodCall = Expression.Call(null, method, new Expression[] { nameParam });
            return Expression.Lambda<Func<string, object>>(methodCall, new[] { nameParam }).Compile();
        }

        public class Log4NetLogger : ILog
        {
            private readonly dynamic _logger;

            internal Log4NetLogger(dynamic logger)
            {
                _logger = logger;
            }

            public bool Log(Level logLevel, Func<string> messageFunc)
            {
                if (messageFunc == null)
                {
                    return IsLogLevelEnable(logLevel);
                }
                switch (logLevel)
                {
                    case Level.Info:
                        if (_logger.IsInfoEnabled)
                        {
                            _logger.Info(messageFunc());
                            return true;
                        }
                        break;
                    case Level.Warn:
                        if (_logger.IsWarnEnabled)
                        {
                            _logger.Warn(messageFunc());
                            return true;
                        }
                        break;
                    case Level.Error:
                        if (_logger.IsErrorEnabled)
                        {
                            _logger.Error(messageFunc());
                            return true;
                        }
                        break;
                    case Level.Fatal:
                        if (_logger.IsFatalEnabled)
                        {
                            _logger.Fatal(messageFunc());
                            return true;
                        }
                        break;
                    default:
                        if (_logger.IsDebugEnabled)
                        {
                            _logger.Debug(messageFunc()); // Log4Net doesn't have a 'Trace' level, so all Trace messages are written as 'Debug'
                            return true;
                        }
                        break;
                }
                return false;
            }

            public void Log<TException>(Level logLevel, Func<string> messageFunc, TException exception)
                where TException : Exception
            {
                switch (logLevel)
                {
                    case Level.Info:
                        if (_logger.IsDebugEnabled)
                        {
                            _logger.Info(messageFunc(), exception);
                        }
                        break;
                    case Level.Warn:
                        if (_logger.IsWarnEnabled)
                        {
                            _logger.Warn(messageFunc(), exception);
                        }
                        break;
                    case Level.Error:
                        if (_logger.IsErrorEnabled)
                        {
                            _logger.Error(messageFunc(), exception);
                        }
                        break;
                    case Level.Fatal:
                        if (_logger.IsFatalEnabled)
                        {
                            _logger.Fatal(messageFunc(), exception);
                        }
                        break;
                    default:
                        if (_logger.IsDebugEnabled)
                        {
                            _logger.Debug(messageFunc(), exception);
                        }
                        break;
                }
            }

            private bool IsLogLevelEnable(Level logLevel)
            {
                switch (logLevel)
                {
                    case Level.Debug:
                        return _logger.IsDebugEnabled;
                    case Level.Info:
                        return _logger.IsInfoEnabled;
                    case Level.Warn:
                        return _logger.IsWarnEnabled;
                    case Level.Error:
                        return _logger.IsErrorEnabled;
                    case Level.Fatal:
                        return _logger.IsFatalEnabled;
                    default:
                        return _logger.IsDebugEnabled;
                }
            }
        }
    }
}
