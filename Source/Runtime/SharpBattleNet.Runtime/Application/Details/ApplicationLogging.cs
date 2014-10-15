using Nini.Config;
using Ninject;
using NLog;
using NLog.Config;
using NLog.Targets;
using SharpBattleNet.Runtime.Utilities.Debugging;
using SharpBattleNet.Runtime.Utilities.Logging;
using SharpBattleNet.Runtime.Utilities.Logging.Providers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpBattleNet.Runtime.Application.Details
{
    internal sealed class ApplicationLogging : IDisposable
    {
        private readonly IKernel _injectionKernel = null;
        private readonly string _applicationName = "";
        private readonly string _writeDirectory = "";

        private bool _disposed = false;

        /// <summary>
        /// Returns a NLog log level depending on the passed in string.
        /// </summary>
        /// <param name="level">
        /// The string to parse for log levels.
        /// </param>
        /// <returns>
        /// An <see cref="LogLevel"/> enumartion converted from the level
        /// argument.
        /// </returns>
        private LogLevel GetLogLevel(string level)
        {
            Guard.AgainstNull(level);
            Guard.AgainstEmptyString(level);

            switch (level)
            {
                case "Trace":
                    return LogLevel.Trace;
                case "Debug":
                    return LogLevel.Debug;
                case "Info":
                    return LogLevel.Info;
                case "Warn":
                    return LogLevel.Warn;
                case "Error":
                    return LogLevel.Error;
                case "Fatal":
                    return LogLevel.Fatal;
            }

            return LogLevel.Off;
        }

        /// <summary>
        /// Configure NLog to output to the console.
        /// </summary>
        /// <param name="config">The NLog configuration object to configure.</param>
        private void ConfigureConsoleLogging(LoggingConfiguration configuration)
        {
            var configSource = _injectionKernel.Get<IConfigSource>();
            var source = configSource.Configs["General"];
            var level = "";
            
            if(null == source)
            {
                level = "Trace";
            }
            else
            {
                level = source.Get("LogConsoleLevel", "Trace");
            }

            // Console logging should always be available
            var consoleTarget = new ColoredConsoleTarget();
            configuration.AddTarget("console", consoleTarget);

            consoleTarget.RowHighlightingRules.Add(new ConsoleRowHighlightingRule("level == LogLevel.Fatal", ConsoleOutputColor.Red, ConsoleOutputColor.NoChange));
            consoleTarget.RowHighlightingRules.Add(new ConsoleRowHighlightingRule("level == LogLevel.Error", ConsoleOutputColor.DarkRed, ConsoleOutputColor.NoChange));
            consoleTarget.RowHighlightingRules.Add(new ConsoleRowHighlightingRule("level == LogLevel.Warn", ConsoleOutputColor.Yellow, ConsoleOutputColor.NoChange));
            consoleTarget.RowHighlightingRules.Add(new ConsoleRowHighlightingRule("level == LogLevel.Info", ConsoleOutputColor.White, ConsoleOutputColor.NoChange));
            consoleTarget.RowHighlightingRules.Add(new ConsoleRowHighlightingRule("level == LogLevel.Debug", ConsoleOutputColor.Gray, ConsoleOutputColor.NoChange));
            consoleTarget.RowHighlightingRules.Add(new ConsoleRowHighlightingRule("level == LogLevel.Trace", ConsoleOutputColor.DarkGray, ConsoleOutputColor.NoChange));

            consoleTarget.UseDefaultRowHighlightingRules = false;
            consoleTarget.Layout = @"${message}";

            var consoleRule = new LoggingRule("*", GetLogLevel(level), consoleTarget);
            configuration.LoggingRules.Add(consoleRule);

            return;
        }

        /// <summary>
        /// Configures the NLog subsystem to log to a file inside the write
        /// directory.
        /// </summary>
        /// <param name="config">The NLog configuration object to configure.</param>
        private void ConfigureFileLogging(LoggingConfiguration configuration)
        {
            var configSource = _injectionKernel.Get<IConfigSource>();
            var source = configSource.Configs["General"];
            var level = "";

            if (null == source)
            {
                level = "Trace";
            }
            else
            {
                level = source.Get("LogFileLevel", "Trace");
            }

            // File logging should always be available
            DateTime currentTime = DateTime.Now;
            string logDate = string.Format("{0}-{1}-{2}-{3}-{4}-{5}", currentTime.Year, currentTime.Month, currentTime.Day, currentTime.Hour, currentTime.Minute, currentTime.Second);
            string logFilename = Path.Combine(_writeDirectory, string.Format("Log-{0}.log", logDate));

            var fileTarget = new FileTarget();
            configuration.AddTarget("file", fileTarget);

            fileTarget.FileName = logFilename;
            fileTarget.Layout = @"${processtime} ${threadid} ${level} ${logger} ${message}";

            var fileRule = new LoggingRule("*", GetLogLevel(level), fileTarget);
            configuration.LoggingRules.Add(fileRule);

            return;
        }

        private void Configure()
        {
            var configuration = new LoggingConfiguration();

            ConfigureConsoleLogging(configuration);
            ConfigureFileLogging(configuration);

            LogManager.Configuration = configuration;

            _injectionKernel.Bind<ILogProvider>().To<NLogLogProvider>().InSingletonScope();

            return;
        }

        public ApplicationLogging(IKernel injectionKernel, string applicationName, string writeDirectory)
        {
            _injectionKernel = injectionKernel;
            _applicationName = applicationName;
            _writeDirectory = writeDirectory;

            Configure();

            return;
        }

        private void Dispose(bool disposing)
        {
            if (false == _disposed)
            {
                if (true == disposing)
                {
                    
                }

                _disposed = true;
            }

            return;
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);

            return;
        }
    }
}
