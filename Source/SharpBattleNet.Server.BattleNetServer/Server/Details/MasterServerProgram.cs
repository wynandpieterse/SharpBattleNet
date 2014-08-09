using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpBattleNet.Server.MasterServer.Server.Details
{
    internal sealed class MasterServerProgram : IMasterServerProgram
    {
        public void Start()
        {
            Console.WriteLine("Hello, World");
            return;
        }

        public void Stop()
        {
            return;
        }
    }
}
