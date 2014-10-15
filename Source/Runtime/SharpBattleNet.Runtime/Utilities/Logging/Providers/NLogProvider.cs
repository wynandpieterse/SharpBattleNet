using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpBattleNet.Runtime.Utilities.Logging.Providers
{
    public class NLogLogProvider : ILogProvider
    {
        private readonly Func<string, object> _getLoggerByNameDelegate;
        private static bool _providerIsAvailableOverride = true;

        public NLogLogProvider()
        {
            if (!IsLoggerAvailable())
            {
                throw new InvalidOperationException("NLog.LogManager not found");
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
            return new NLogLogger(_getLoggerByNameDelegate(name));
        }

        public static bool IsLoggerAvailable()
        {
            return ProviderIsAvailableOverride && GetLogManagerType() != null;
        }

        private static Type GetLogManagerType()
        {
            return Type.GetType("NLog.LogManager, nlog");
        }

        private static Func<string, object> GetGetLoggerMethodCall()
        {
            Type logManagerType = GetLogManagerType();
            MethodInfo method = logManagerType.GetMethod("GetLogger", new[] { typeof(string) });
            ParameterExpression nameParam = Expression.Parameter(typeof(string), "name");
            MethodCallExpression methodCall = Expression.Call(null, method, new Expression[] { nameParam });
            return Expression.Lambda<Func<string, object>>(methodCall, new[] { nameParam }).Compile();
        }

        public class NLogLogger : ILog
        {
            private readonly dynamic _logger;

            internal NLogLogger(dynamic logger)
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
                    case Level.Debug:
                        if (_logger.IsDebugEnabled)
                        {
                            _logger.Debug(messageFunc());
                            return true;
                        }
                        break;
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
                        if (_logger.IsTraceEnabled)
                        {
                            _logger.Trace(messageFunc());
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
                    case Level.Debug:
                        if (_logger.IsDebugEnabled)
                        {
                            _logger.DebugException(messageFunc(), exception);
                        }
                        break;
                    case Level.Info:
                        if (_logger.IsInfoEnabled)
                        {
                            _logger.InfoException(messageFunc(), exception);
                        }
                        break;
                    case Level.Warn:
                        if (_logger.IsWarnEnabled)
                        {
                            _logger.WarnException(messageFunc(), exception);
                        }
                        break;
                    case Level.Error:
                        if (_logger.IsErrorEnabled)
                        {
                            _logger.ErrorException(messageFunc(), exception);
                        }
                        break;
                    case Level.Fatal:
                        if (_logger.IsFatalEnabled)
                        {
                            _logger.FatalException(messageFunc(), exception);
                        }
                        break;
                    default:
                        if (_logger.IsTraceEnabled)
                        {
                            _logger.TraceException(messageFunc(), exception);
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
                        return _logger.IsTraceEnabled;
                }
            }
        }
    }
}
