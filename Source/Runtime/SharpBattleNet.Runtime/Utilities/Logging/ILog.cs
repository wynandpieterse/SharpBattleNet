using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpBattleNet.Runtime.Utilities.Logging
{
    /// <summary>
    /// Simple interface that represent a logger.
    /// </summary>
    public interface ILog
    {
        /// <summary>
        /// Log a message the specified log level.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="messageFunc">The message function.</param>
        /// <remarks>
        /// Note to implementors: the message func should not be called if the loglevel is not enabled
        /// so as not to incur perfomance penalties.
        /// </remarks>
        bool Log(Level logLevel, Func<string> messageFunc);

        /// <summary>
        /// Log a message and exception at the specified log level.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="logLevel">The log level.</param>
        /// <param name="messageFunc">The message function.</param>
        /// <param name="exception">The exception.</param>
        /// <remarks>
        /// Note to implementors: the message func should not be called if the loglevel is not enabled
        /// so as not to incur perfomance penalties.
        /// </remarks>
        void Log<TException>(Level logLevel, Func<string> messageFunc, TException exception) where TException : Exception;
    }
}
