using Nini.Config;
using Ninject;
using Ninject.Modules;
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

            if(false == File.Exists(configurationPath))
            {
                File.Copy(configurationBasePath, configurationPath);
            }

            _injectionKernel.Bind<IConfigSource>().ToConstant(new IniConfigSource(configurationPath)).InSingletonScope();

            return;
        }

        private void ConfigureLogging()
        {


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
