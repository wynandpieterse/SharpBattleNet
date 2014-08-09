using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpBattleNet.Server.MasterServer.Server
{
    public interface IMasterServerProgram
    {
        void Start();
        void Stop();
    }
}
