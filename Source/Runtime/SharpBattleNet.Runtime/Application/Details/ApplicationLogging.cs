#region Header
//
//    _  _   ____        _   _   _         _   _      _   
//  _| || |_| __ )  __ _| |_| |_| | ___   | \ | | ___| |_ 
// |_  .. _ |  _ \ / _` | __| __| |/ _ \  |  \| |/ _ \ __|
// |_      _| |_) | (_| | |_| |_| |  __/_ | |\  |  __/ |_ 
//   |_||_| |____/ \__,_|\__|\__|_|\___(_)_ | \_|\___|\__|
//
// The MIT License
// 
// Copyright(c) 2014 Wynand Pieters. https://github.com/wpieterse/SharpBattleNet

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
#endregion

namespace SharpBattleNet.Runtime.Application.Details
{
    #region Usings
    using System;
    using System.IO;

    using Ninject;

    using Nini;
    using Nini.Config;

    using NLog;
    using NLog.Config;
    using NLog.Targets;

    using SharpBattleNet;
    using SharpBattleNet.Runtime;
    using SharpBattleNet.Runtime.Utilities;
    using SharpBattleNet.Runtime.Utilities.Debugging;
    using SharpBattleNet.Runtime.Utilities.Logging;
    using SharpBattleNet.Runtime.Utilities.Logging.Providers;
    #endregion

    internal sealed class ApplicationLogging : IDisposable
    {
        private readonly IKernel _injectionKernel = null;
        private readonly string _applicationName = "";
        private readonly string _writeDirectory = "";

        private bool _disposed = false;

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

        protected virtual void Dispose(bool disposing)
        {
            if (false == _disposed)
            {
                if (true == disposing)
                {
                    // Dispose managed resources
                }

                // Dispose unmanaged resources
            }

            _disposed = true;

            // Call base dispose

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
