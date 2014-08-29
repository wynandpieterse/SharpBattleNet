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

namespace SharpBattleNet.Framework
{
    #region Usings
    using System;
    using System.IO;
    using Ninject;
    using Ninject.Modules;
    using Nini.Config;
    using NLog;
    using NLog.Targets;
    using NLog.Config;
    using SharpBattleNet.Framework.Utilities.Debugging;
    using Ninject.Extensions.Factory;
    #endregion

    /// <summary>
    /// Provides common IoC modules that is used by most of the system. Provides
    /// configuration and logging modules built into one.
    /// </summary>
    public sealed class FrameworkModule : NinjectModule
    {
        private readonly string _applicationName = "";

        private string _writeDirectory = "";
        private bool _writeDirectorySuccessfull = false;

        /// <summary>
        /// Constructs an empty <see cref="FrameworkModule"/>.
        /// </summary>
        /// <param name="applicationName">
        /// The application name that is calling us.
        /// </param>
        public FrameworkModule(string applicationName)
        {
            Guard.AgainstNull(applicationName);
            Guard.AgainstEmptyString(applicationName);

            _applicationName = applicationName;

            return;
        }

        /// <summary>
        /// Creates the application write directory if it does not exists.
        /// </summary>
        private void ConfigureWriteDirectory()
        {
            try
            {
                _writeDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "SharpBattleNet");
                _writeDirectory = Path.Combine(_writeDirectory, _applicationName);

                Directory.CreateDirectory(_writeDirectory);

                _writeDirectorySuccessfull = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to create write directory for {0}", _applicationName);
                Console.WriteLine(ex.Message);

                _writeDirectorySuccessfull = false;
            }

            return;
        }

        /// <summary>
        /// Configures the configuration subsystem that can be used by modules
        /// to make themself configurable to the open world.
        /// </summary>
        private void ConfigureConfiguration()
        {
            string configurationDirectory = Path.Combine(_writeDirectory, "Configuration");
            string configurationFilename = Path.Combine(configurationDirectory, string.Format("{0}.ini", _applicationName));

            if (true == _writeDirectorySuccessfull)
            {
                Directory.CreateDirectory(configurationDirectory);

                // check to see if configuration file already exist there
                if (false == File.Exists(configurationFilename))
                {
                    try
                    {
                        // copy configuration file over
                        File.Copy(string.Format("../Configuration/{0}.ini", _applicationName), configurationFilename);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Failed to copy over configuration to the specified application directory. Using default!!!");
                        Console.WriteLine(ex.Message);

                        configurationFilename = string.Format("../Configuration/{0}.ini");
                    }
                }
            }
            else
            {
                // If we failed to create the write directory, use the default configuration.
                configurationFilename = string.Format("../Configuration/{0}.ini", _applicationName);
            }

            Bind<IConfigSource>().ToMethod(context => new IniConfigSource(configurationFilename)).InSingletonScope();

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
        private void ConfigureConsoleLogging(LoggingConfiguration config)
        {
            Guard.AgainstNull(config);

            var configSource = Kernel.Get<IConfigSource>();
            var source = configSource.Configs["General"];
            if(null != source)
            {
                if(true == source.GetBoolean("LogConsole"))
                {
                    var consoleTarget = new ColoredConsoleTarget();
                    config.AddTarget("console", consoleTarget);

                    consoleTarget.Layout = @"${date:format=HH\\:MM\\:ss} ${message}";

                    var consoleRule = new LoggingRule("*", GetLogLevel(source.Get("LogConsoleLevel")), consoleTarget);
                    config.LoggingRules.Add(consoleRule);
                }
            }

            return;
        }

        /// <summary>
        /// Configures the NLog subsystem to log to a file inside the write
        /// directory.
        /// </summary>
        /// <param name="config">The NLog configuration object to configure.</param>
        private void ConfigureFileLogging(LoggingConfiguration config)
        {
            Guard.AgainstNull(config);

            if (true == _writeDirectorySuccessfull)
            {
                string logDirectory = Path.Combine(_writeDirectory, "Logs");
                DateTime currentTime = DateTime.Now;
                string logDate = string.Format("{0}-{1}-{2}-{3}-{4}-{5}", currentTime.Year, currentTime.Month, currentTime.Day, currentTime.Hour, currentTime.Minute, currentTime.Second);
                string logFilename = Path.Combine(logDirectory, string.Format("Log-{0}.log", logDate));

                try
                {
                    Directory.CreateDirectory(logDirectory);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to create log directory. File logs will not be written");
                    Console.WriteLine(ex.Message);

                    return;
                }

                var fileTarget = new FileTarget();
                config.AddTarget("file", fileTarget);

                fileTarget.FileName = logFilename;
                fileTarget.Layout = @"${date:format=HH\\:MM\\:ss} ${logger} ${message}";

                var fileRule = new LoggingRule("*", LogLevel.Debug, fileTarget);
                config.LoggingRules.Add(fileRule);
            }

            return;
        }

        /// <summary>
        /// Configures NLog.
        /// </summary>
        private void ConfigureLogging()
        {
            var config = new LoggingConfiguration();

            ConfigureConsoleLogging(config);
            ConfigureFileLogging(config);

            LogManager.Configuration = config;

            return;
        }

        /// <summary>
        /// Called by Ninject to configure this module.
        /// </summary>
        public override void Load()
        {
            ConfigureWriteDirectory();
            ConfigureConfiguration();
            ConfigureLogging();

            return;
        }
    }
}

