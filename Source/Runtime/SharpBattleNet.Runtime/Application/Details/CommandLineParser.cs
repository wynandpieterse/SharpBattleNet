using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpBattleNet.Runtime.Application.Details
{
    internal sealed class CommandLineParser : ICommandLineParser
    {
        private readonly IKernel _injectionKernel = null;
        private readonly string _applicationName = "";
        private readonly string _writeDirectory = "";

        private bool _disposed = false;

        public CommandLineParser(IKernel injectionKernel, string applicationName, string writeDirectory)
        {
            _injectionKernel = injectionKernel;
            _applicationName = applicationName;
            _writeDirectory = writeDirectory;

            return;
        }

        public void Parse(string[] arguments)
        {
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
