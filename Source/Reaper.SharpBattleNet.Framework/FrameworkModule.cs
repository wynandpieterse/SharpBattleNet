namespace Reaper.SharpBattleNet.Framework
{
    using System;
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

    public sealed class FrameworkModule : NinjectModule
    {
        private readonly string _configurationFile = "";

        public FrameworkModule(string configurationFile)
        {
            _configurationFile = configurationFile;

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
            var fileTarget = new FileTarget();
            config.AddTarget("file", fileTarget);

            fileTarget.FileName = String.Format("{0}Log{1}.log", GetLogLevel(source.Get("LogFileLevel")), DateTime.Now);
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
            ConfigureConfiguration();
            ConfigureLogging();

            return;
        }
    }
}

