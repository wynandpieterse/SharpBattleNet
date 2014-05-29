namespace Reaper.SharpBattleNet.Framework
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

    using Reaper;
    using Reaper.SharpBattleNet;
    using Reaper.SharpBattleNet.Framework;
    using Reaper.SharpBattleNet.Framework.Extensions;

    public sealed class FrameworkModule : NinjectModule
    {
        private readonly string _configurationFile = "";

        private string _writeDirectory = "";

        public FrameworkModule(string configurationFile)
        {
            _configurationFile = configurationFile;

            return;
        }

        private void ConfigureWriteDirectory()
        {
            Assembly entryAssembly = null;

            entryAssembly = Assembly.GetEntryAssembly();

            _writeDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "SharpBattleNet");
            _writeDirectory = Path.Combine(_writeDirectory, entryAssembly.GetAssemblyProduct());

            // Check for exceptions here, please
            Directory.CreateDirectory(_writeDirectory);

            return;
        }

        private void ConfigureConfiguration()
        {
            Bind<IConfigSource>().ToMethod<IniConfigSource>(context => new IniConfigSource(_configurationFile)).InSingletonScope();

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

