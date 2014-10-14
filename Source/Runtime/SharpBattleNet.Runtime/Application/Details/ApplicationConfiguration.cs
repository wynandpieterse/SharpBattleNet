using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpBattleNet.Runtime.Application.Details
{
    internal sealed class ApplicationConfiguration : IApplicationConfiguration
    {
        private bool _disposed = false;

        public void Configure(string applicationName, string writeDirectory)
        {
            throw new NotImplementedException();
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
