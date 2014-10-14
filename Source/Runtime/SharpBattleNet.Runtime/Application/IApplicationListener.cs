using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpBattleNet.Runtime.Application
{
    public interface IApplicationListener : IDisposable
    {
        int Run();
    }
}
