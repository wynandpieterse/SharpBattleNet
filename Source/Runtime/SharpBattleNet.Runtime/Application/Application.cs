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
        private List<INinjectModule> _injectionModules = null;
        private string _writeDirectory = null;

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
            Directory.CreateDirectory(_writeDirectory);

            return;
        }

        private void ConfigureConsole()
        {
            var currentAssembly = Assembly.GetEntryAssembly();

            Console.Title = string.Format("{0} - {1}", currentAssembly.GetAssemblyTitle(), currentAssembly.GetAssemblyFileVersion());
            Console.WindowWidth = 120;
            Console.WindowHeight = 40;

            return;
        }

        public int UnguardedRun()
        {
            using(var injectionKernel = new StandardKernel())
            {
                ConfigureConsole();
                ConfigureWriteDirectory();

                injectionKernel.Load(_injectionModules);

                using(var configuration = new ApplicationConfiguration(injectionKernel, _name, _writeDirectory))
                {
                    using(var logging = new ApplicationLogging(injectionKernel, _name, _writeDirectory))
                    {
                        using (var commandLine = new ApplicationParser(injectionKernel, _arguments))
                        {
                            using(var application = injectionKernel.Get<IApplicationListener>())
                            {
                                return application.Run();
                            }
                        }
                    }
                }
            }
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
