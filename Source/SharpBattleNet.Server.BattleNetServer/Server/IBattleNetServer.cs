using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpBattleNet.Server.BattleNetServer.Server
{
    public interface IBattleNetServer
    {
        void Start();
        void Stop();
    }
}
