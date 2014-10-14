using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpBattleNet.Runtime.Application
{
    internal interface IApplicationLogging : IDisposable
    {
        void Configure(IKernel injectionKernel, string applicationName, string writeDirectory);
    }
}
