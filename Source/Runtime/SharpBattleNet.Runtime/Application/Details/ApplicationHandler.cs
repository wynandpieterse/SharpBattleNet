using SharpBattleNet.Runtime.Utilities.Debugging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpBattleNet.Runtime.Application.Details
{
    internal abstract class ApplicationHandler : IApplicationHandler
    {
        private ICommandLineParser _commandLineParser = null;
        private IApplicationListener _applicationListener = null;

        private bool _disposed = false;

        public ApplicationHandler(ICommandLineParser commandLineParser, IApplicationListener applicationListener)
        {
            Guard.AgainstNull(commandLineParser);
            Guard.AgainstNull(applicationListener);

            _commandLineParser = commandLineParser;
            _applicationListener = applicationListener;

            return;
        }

        public abstract int Run(string[] arguments);

        protected virtual void Start()
        {
            _applicationListener.Start();

            return;
        }

        protected virtual void Stop()
        {
            _applicationListener.Stop();

            return;
        }

        protected virtual void Dispose(bool disposing)
        {
            if(false == _disposed)
            {
                if(true == disposing)
                {
                    _applicationListener.Dispose();
                    _applicationListener = null;
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
