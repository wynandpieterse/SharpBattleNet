using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace SharpBattleNet.Framework.Networking.Connection.TCP
{
    public interface IConnectableTCPConnection : ITCPConnection
    {
        void Start(EndPoint address, Func<bool> connected);
    }
}
