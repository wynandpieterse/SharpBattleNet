namespace SharpBattleNet.Framework
{
    using System;
    using System.Reflection;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Ninject;
    using Ninject.Modules;
    using Ninject.Activation;

    using Nini;
    using Nini.Config;
    using Nini.Ini;
    using Nini.Util;

    using NLog;
    using NLog.Targets;
    using NLog.Config;

    using SharpBattleNet;
    using SharpBattleNet.Framework;
    using SharpBattleNet.Framework.Extensions;

    public sealed class FrameworkModule : NinjectModule
    {
        private readonly string _applicationName = "";

        private string _writeDirectory = "";

        public FrameworkModule(string applicationName)
        {
            _applicationName = applicationName;

            return;
        }

        private void ConfigureWriteDirectory()
        {
            Assembly entryAssembly = null;

            entryAssembly = Assembly.GetEntryAssembly();

            _writeDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "SharpBattleNet");
            _writeDirectory = Path.Combine(_writeDirectory, _applicationName);

            // Check for exceptions here, please
            Directory.CreateDirectory(_writeDirectory);

            return;
        }

        private void ConfigureConfiguration()
        {
            string configurationDirectory = Path.Combine(_writeDirectory, "Configuration");
            string configurationFilename = Path.Combine(configurationDirectory, String.Format("{0}.ini", _applicationName));

            // Check for exceptions here, please
            Directory.CreateDirectory(configurationDirectory);

            // check to see if configuration file already exist there
            if(false == File.Exists(configurationFilename))
            {
                // copy configuration file over
                File.Copy(String.Format("../../../Configuration/{0}.ini", _applicationName), configurationFilename);
            }

            Bind<IConfigSource>().ToMethod<IniConfigSource>(context => new IniConfigSource(configurationFilename)).InSingletonScope();

            return;
        }

        private LogLevel GetLogLevel(string level)
        {
            switch(level)
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
            string logDirectory = Path.Combine(_writeDirectory, "Logs");
            DateTime currentTime = DateTime.Now;
            string logDate = String.Format("{0}-{1}-{2}-{3}-{4}-{5}", currentTime.Year, currentTime.Month, currentTime.Day, currentTime.Hour, currentTime.Minute, currentTime.Second);
            string logFilename = Path.Combine(logDirectory, String.Format("Log-{0}.log", logDate));

            // Check for exceptions here please
            Directory.CreateDirectory(logDirectory);

            var fileTarget = new FileTarget();
            config.AddTarget("file", fileTarget);

            fileTarget.FileName = logFilename;
            fileTarget.Layout = @"${date:format=HH\\:MM\\:ss} ${logger} ${message}";

            var fileRule = new LoggingRule("*", LogLevel.Debug, fileTarget);
            config.LoggingRules.Add(fileRule);

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
            ConfigureWriteDirectory();
            ConfigureConfiguration();
            ConfigureLogging();

            return;
        }
    }
}

