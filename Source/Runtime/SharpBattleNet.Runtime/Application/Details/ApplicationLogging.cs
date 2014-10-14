﻿using Nini.Config;
using Ninject;
using NLog;
using NLog.Config;
using NLog.Targets;
using SharpBattleNet.Runtime.Utilities.Debugging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpBattleNet.Runtime.Application.Details
{
    internal sealed class ApplicationLogging : IApplicationLogging
    {
        private bool _disposed = false;

        private IKernel _injectionKernel = null;
        private LoggingConfiguration _loggingConfiguration = null;
        private string _applicationName = "";
        private string _writeDirectory = "";

        public ApplicationLogging()
        {
            return;
        }

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
        private void ConfigureConsoleLogging()
        {
            var configSource = _injectionKernel.Get<IConfigSource>();
            var source = configSource.Configs["General"];

            if (null != source)
            {
                if (true == source.GetBoolean("LogConsole"))
                {
                    var consoleTarget = new ColoredConsoleTarget();
                    _loggingConfiguration.AddTarget("console", consoleTarget);

                    consoleTarget.RowHighlightingRules.Add(new ConsoleRowHighlightingRule("level == LogLevel.Fatal", ConsoleOutputColor.Red, ConsoleOutputColor.NoChange));
                    consoleTarget.RowHighlightingRules.Add(new ConsoleRowHighlightingRule("level == LogLevel.Error", ConsoleOutputColor.DarkRed, ConsoleOutputColor.NoChange));
                    consoleTarget.RowHighlightingRules.Add(new ConsoleRowHighlightingRule("level == LogLevel.Warn", ConsoleOutputColor.Yellow, ConsoleOutputColor.NoChange));
                    consoleTarget.RowHighlightingRules.Add(new ConsoleRowHighlightingRule("level == LogLevel.Info", ConsoleOutputColor.White, ConsoleOutputColor.NoChange));
                    consoleTarget.RowHighlightingRules.Add(new ConsoleRowHighlightingRule("level == LogLevel.Debug", ConsoleOutputColor.Gray, ConsoleOutputColor.NoChange));
                    consoleTarget.RowHighlightingRules.Add(new ConsoleRowHighlightingRule("level == LogLevel.Trace", ConsoleOutputColor.DarkGray, ConsoleOutputColor.NoChange));

                    consoleTarget.UseDefaultRowHighlightingRules = false;
                    consoleTarget.Layout = @"${message}";

                    var consoleRule = new LoggingRule("*", GetLogLevel(source.Get("LogConsoleLevel")), consoleTarget);
                    _loggingConfiguration.LoggingRules.Add(consoleRule);
                }
            }

            return;
        }

        /// <summary>
        /// Configures the NLog subsystem to log to a file inside the write
        /// directory.
        /// </summary>
        /// <param name="config">The NLog configuration object to configure.</param>
        private void ConfigureFileLogging()
        {
            var configSource = _injectionKernel.Get<IConfigSource>();
            var source = configSource.Configs["General"];

            if (null != source)
            {
                if (true == source.GetBoolean("LogFile"))
                {
                    DateTime currentTime = DateTime.Now;
                    string logDate = string.Format("{0}-{1}-{2}-{3}-{4}-{5}", currentTime.Year, currentTime.Month, currentTime.Day, currentTime.Hour, currentTime.Minute, currentTime.Second);
                    string logDirectory = Path.Combine(_writeDirectory, "Logs");
                    string logFilename = Path.Combine(_writeDirectory, string.Format("Log-{0}.log", logDate));

                    Directory.CreateDirectory(logDirectory);

                    var fileTarget = new FileTarget();
                    _loggingConfiguration.AddTarget("file", fileTarget);

                    fileTarget.FileName = logFilename;
                    fileTarget.Layout = @"${processtime} ${threadid} ${level} ${logger} ${message}";

                    var fileRule = new LoggingRule("*", GetLogLevel(source.Get("LogFileLevel")), fileTarget);
                    _loggingConfiguration.LoggingRules.Add(fileRule);
                }
            }

            return;
        }

        public void Configure(IKernel injectionKernel, string applicationName, string writeDirectory)
        {
            _loggingConfiguration = new LoggingConfiguration();

            ConfigureConsoleLogging();
            ConfigureFileLogging();

            LogManager.Configuration = _loggingConfiguration;

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
