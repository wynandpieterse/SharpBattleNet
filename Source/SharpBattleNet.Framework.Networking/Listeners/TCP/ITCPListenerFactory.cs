using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpBattleNet.Framework.Networking.Listeners.TCP
{
    public interface ITCPListenerFactory
    {
        ITCPListener Create();
    }
}
