namespace SharpBattleNet.Framework
{
    using System;
    using System.IO;

    using Ninject;
    using Ninject.Modules;
    using Nini.Config;

    using NLog;
    using NLog.Targets;
    using NLog.Config;

    using SharpBattleNet.Framework.Utilities.Debugging;

    public sealed class FrameworkModule : NinjectModule
    {
        private readonly string _applicationName = "";

        private string _writeDirectory = "";
        private bool _writeDirectorySuccessfull = false;

        public FrameworkModule(string applicationName)
        {
            Guard.AgainstNull(applicationName);
            Guard.AgainstEmptyString(applicationName);

            _applicationName = applicationName;

            return;
        }

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
                        File.Copy(string.Format("../../../Configuration/{0}.ini", _applicationName), configurationFilename);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Failed to copy over configuration to the specified application directory. Using default!!!");
                        Console.WriteLine(ex.Message);

                        configurationFilename = string.Format("../../../Configuration/{0}.ini");
                    }
                }
            }
            else
            {
                // If we failed to create the write directory, use the default configuration.
                configurationFilename = string.Format("../../../Configuration/{0}.ini", _applicationName);
            }

            Bind<IConfigSource>().ToMethod(context => new IniConfigSource(configurationFilename)).InSingletonScope();

            return;
        }

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

        private void ConfigureLogging()
        {
            var config = new LoggingConfiguration();

            ConfigureConsoleLogging(config);
            ConfigureFileLogging(config);

            LogManager.Configuration = config;

            return;
        }

        public override void Load()
        {
            try
            {
                ConfigureWriteDirectory();
                ConfigureConfiguration();
                ConfigureLogging();
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to configure framework. Can't continue. See internal exception for more details", ex);
            }

            return;
        }
    }
}

