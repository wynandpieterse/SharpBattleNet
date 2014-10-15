using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpBattleNet.Runtime.Utilities.Logging
{
    public class LoggerExecutionWrapper : ILog
    {
        private readonly ILog _logger;
        public const string FailedToGenerateLogMessage = "Failed to generate log message";

        public ILog WrappedLogger
        {
            get { return _logger; }
        }

        public LoggerExecutionWrapper(ILog logger)
        {
            _logger = logger;
        }

        public bool Log(Level logLevel, Func<string> messageFunc)
        {
            Func<string> wrappedMessageFunc = () =>
            {
                try
                {
                    return messageFunc();
                }
                catch (Exception ex)
                {
                    Log(Level.Error, () => FailedToGenerateLogMessage, ex);
                }
                return null;
            };
            return _logger.Log(logLevel, wrappedMessageFunc);
        }

        public void Log<TException>(Level logLevel, Func<string> messageFunc, TException exception) where TException : Exception
        {
            Func<string> wrappedMessageFunc = () =>
            {
                try
                {
                    return messageFunc();
                }
                catch (Exception ex)
                {
                    Log(Level.Error, () => FailedToGenerateLogMessage, ex);
                }
                return null;
            };
            _logger.Log(logLevel, wrappedMessageFunc, exception);
        }
    }
}
