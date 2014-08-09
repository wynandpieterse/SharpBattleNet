using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpBattleNet.Server.DiabloIIRealmServer.Server
{
    public interface IDiabloIIRealmServerProgram
    {
        void Start();
        void Stop();
    }
}
