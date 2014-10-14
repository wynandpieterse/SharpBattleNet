using Nini.Config;
using Ninject;
using Ninject.Modules;
using NLog;
using NLog.Config;
using NLog.Targets;
using SharpBattleNet.Runtime.Application.Details;
using SharpBattleNet.Runtime.Utilities.Debugging;
using SharpBattleNet.Runtime.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SharpBattleNet.Runtime.Application
{
    public sealed class Application : IDisposable
    {
        private readonly string _name = "";
        private readonly string[] _arguments = null;

        private bool _disposed = false;

        private IKernel _injectionKernel = null;
        private List<INinjectModule> _injectionModules = null;
        private IApplicationListener _application = null;

        private bool _configuredWriteDirectory = false;
        private string _writeDirectory = null;
        private string _writeLogDirectory = null;

        public Application(string name, string[] arguments)
        {
            Guard.AgainstEmptyString(name);
            Guard.AgainstNull(arguments);

            _name = name;
            _arguments = arguments;

            _injectionModules = new List<INinjectModule>();

            return;
        }

        public void AddDependencyModule(NinjectModule module)
        {
            if(true == _injectionModules.Contains(module))
            {
                return;
            }

            _injectionModules.Add(module);
            return;
        }

        private void ConfigureWriteDirectory()
        {
            string userApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

            string battleNetDirectory = Path.Combine(userApplicationData, "SharpBattleNet");
            string applicationDirectory = Path.Combine(battleNetDirectory, _name);

            _writeDirectory = applicationDirectory;
            _writeLogDirectory = Path.Combine(applicationDirectory, "Logs");

            Directory.CreateDirectory(_writeLogDirectory);

            _configuredWriteDirectory = true;

            return;
        }

        private void ConfigureConfiguration()
        {
            string configurationFile = _name + ".ini";
            string configurationBasePath = "../Configuration/" + configurationFile;
            string configurationPath = Path.Combine(_writeDirectory, configurationFile);
            DateTime configurationBaseTime = default(DateTime);
            DateTime configurationTime = default(DateTime);

            if(false == File.Exists(configurationPath))
            {
                File.Copy(configurationBasePath, configurationPath);
            }
            else
            {
                configurationBaseTime = File.GetLastWriteTimeUtc(configurationBasePath);
                configurationTime = File.GetLastWriteTimeUtc(configurationPath);

                if(configurationBaseTime > configurationTime)
                {
                    File.Delete(configurationPath);
                    File.Copy(configurationBasePath, configurationPath);
                }
            }

            _injectionKernel.Bind<IConfigSource>().ToConstant(new IniConfigSource(configurationPath)).InSingletonScope();

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

            var configSource = _injectionKernel.Get<IConfigSource>();
            var source = configSource.Configs["General"];

            if (null != source)
            {
                if (true == source.GetBoolean("LogConsole"))
                {
                    var consoleTarget = new ColoredConsoleTarget();
                    config.AddTarget("console", consoleTarget);

                    consoleTarget.RowHighlightingRules.Add(new ConsoleRowHighlightingRule("level == LogLevel.Fatal", ConsoleOutputColor.Red, ConsoleOutputColor.NoChange));
                    consoleTarget.RowHighlightingRules.Add(new ConsoleRowHighlightingRule("level == LogLevel.Error", ConsoleOutputColor.DarkRed, ConsoleOutputColor.NoChange));
                    consoleTarget.RowHighlightingRules.Add(new ConsoleRowHighlightingRule("level == LogLevel.Warn", ConsoleOutputColor.Yellow, ConsoleOutputColor.NoChange));
                    consoleTarget.RowHighlightingRules.Add(new ConsoleRowHighlightingRule("level == LogLevel.Info", ConsoleOutputColor.White, ConsoleOutputColor.NoChange));
                    consoleTarget.RowHighlightingRules.Add(new ConsoleRowHighlightingRule("level == LogLevel.Debug", ConsoleOutputColor.Gray, ConsoleOutputColor.NoChange));
                    consoleTarget.RowHighlightingRules.Add(new ConsoleRowHighlightingRule("level == LogLevel.Trace", ConsoleOutputColor.DarkGray, ConsoleOutputColor.NoChange));

                    consoleTarget.UseDefaultRowHighlightingRules = false;
                    consoleTarget.Layout = @"${message}";

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

            DateTime currentTime = DateTime.Now;
            string logDate = string.Format("{0}-{1}-{2}-{3}-{4}-{5}", currentTime.Year, currentTime.Month, currentTime.Day, currentTime.Hour, currentTime.Minute, currentTime.Second);
            string logFilename = Path.Combine(_writeLogDirectory, string.Format("Log-{0}.log", logDate));

            var fileTarget = new FileTarget();
            config.AddTarget("file", fileTarget);

            fileTarget.FileName = logFilename;
            fileTarget.Layout = @"${processtime} ${threadid} ${level} ${logger} ${message}";

            var fileRule = new LoggingRule("*", LogLevel.Debug, fileTarget);
            config.LoggingRules.Add(fileRule);

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

        private void SetupNinject()
        {
            _injectionKernel = new StandardKernel();

            ConfigureWriteDirectory();
            ConfigureConfiguration();
            ConfigureLogging();

            _injectionKernel.Load(_injectionModules);

            return;
        }

        private void SetupCommandLineParser()
        {
            _injectionKernel.Bind<ICommandLineParser>().To<CommandLineParser>().InSingletonScope();
            return;
        }

        private void SetupApplication()
        {
            var currentAssembly = Assembly.GetEntryAssembly();

            Console.Title = string.Format("{0} - {1}", currentAssembly.GetAssemblyTitle(), currentAssembly.GetAssemblyFileVersion());
            Console.WindowWidth = 120;
            Console.WindowHeight = 40;

            _application = _injectionKernel.Get<IApplicationListener>();

            return;
        }

        private int RunCommandLoop()
        {
            return _application.Run();
        }

        public int UnguardedRun()
        {
            SetupNinject();
            SetupCommandLineParser();
            SetupApplication();

            return RunCommandLoop();
        }

        private void UnhandledException(Exception ex)
        {
            return;
        }

        private void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            UnhandledException((Exception)e.ExceptionObject);

            return;
        }

        private int GuardedRun()
        {
            try
            {
                AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;

                return UnguardedRun();
            }
            catch(Exception ex)
            {
                UnhandledException(ex);
            }

            return -1;
        }

        public int Run()
        {
            #if DEBUG
                return UnguardedRun();
            #else
                return GuardedRun();
            #endif
        }

        private void Dispose(bool disposing)
        {
            if(false == _disposed)
            {
                if(true == disposing)
                {
                    _application.Dispose();
                    _application = null;

                    _injectionKernel.Dispose();
                    _injectionKernel = null;
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
