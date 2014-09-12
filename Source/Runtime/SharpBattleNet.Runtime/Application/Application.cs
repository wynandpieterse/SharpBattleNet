using Ninject;
using Ninject.Modules;
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

            // Add default framework modules
            _injectionModules.Add(new FrameworkModule(_name));

            _injectionKernel.Load(_injectionModules);

            return;
        }

        public int Run()
        {
            return 0;
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
