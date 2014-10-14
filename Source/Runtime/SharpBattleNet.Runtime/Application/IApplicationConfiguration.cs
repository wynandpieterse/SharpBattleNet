using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpBattleNet.Runtime.Application.Details
{
    internal interface IApplicationConfiguration : IDisposable
    {
        void Configure(string applicationName, string writeDirectory);
    }
}
