using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpBattleNet.Runtime.Application.Details
{
    internal sealed class CommandLineParser : IDisposable
    {
        private readonly IKernel _injectionKernel = null;
        private readonly string[] _arguments = null;

        private bool _disposed = false;

        private void Parse()
        {
            return;
        }

        public CommandLineParser(IKernel injectionKernel, string[] arguments)
        {
            _injectionKernel = injectionKernel;
            _arguments = arguments;

            return;
        }

        private void Dispose(bool disposing)
        {
            if (false == _disposed)
            {
                if (true == disposing)
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
