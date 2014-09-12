using Ninject;
using Ninject.Modules;
using SharpBattleNet.Runtime.Application.Details;
using SharpBattleNet.Runtime.Application.Details.Console;
using SharpBattleNet.Runtime.Application.Details.GUI;
using SharpBattleNet.Runtime.Application.Details.Service;
using SharpBattleNet.Runtime.Utilities.Debugging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpBattleNet.Runtime.Application
{
    public sealed class Application : IDisposable
    {
        private readonly ApplicationMode _mode = ApplicationMode.Undefined;
        private readonly string _name = "";
        private readonly string[] _arguments = null;

        private bool _disposed = false;

        private IKernel _injectionKernel = null;
        private List<INinjectModule> _injectionModules = null;

        private IApplicationHandler _applicationHandler = null;

        public Application(ApplicationMode mode, string name, string[] arguments)
        {
            Guard.AgainstEmptyString(name);
            Guard.AgainstNull(arguments);

            _mode = mode;
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

        private void SetupNinject()
        {
            _injectionKernel = new StandardKernel();

            // Add standard application modules
            _injectionKernel.Load(new ConfigurationModule(_name));
            _injectionKernel.Load(new LoggingModule(_name));

            _injectionKernel.Load(_injectionModules);

            return;
        }

        private void SetupCommandLineParser()
        {
            _injectionKernel.Bind<ICommandLineParser>().To<CommandLineParser>().InSingletonScope();
            return;
        }

        private void SetupApplicationHandler()
        {
            switch(_mode)
            {
                case ApplicationMode.Console:
                    _injectionKernel.Bind<IApplicationHandler>().To<ConsoleApplicationHandler>().InSingletonScope();;
                    break;
                case ApplicationMode.GUI:
                    _injectionKernel.Bind<IApplicationHandler>().To<GUIApplicationHandler>().InSingletonScope();
                    break;
                case ApplicationMode.Service:
                    _injectionKernel.Bind<IApplicationHandler>().To<ServiceApplicationHandler>().InSingletonScope();
                    break;
                default:
                    break;
            }

            _applicationHandler = _injectionKernel.Get<IApplicationHandler>();

            return;
        }

        public int UnguardedRun()
        {
            SetupNinject();
            SetupCommandLineParser();
            SetupApplicationHandler();

            return _applicationHandler.Run(_arguments);
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

        protected void Dispose(bool disposing)
        {
            if(false == _disposed)
            {
                if(true == disposing)
                {
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
