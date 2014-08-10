using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace SharpBattleNet.Framework.Networking.Connection.TCP.Details
{
    internal sealed class ConnectibleTCPConnection : BaseTCPConnection, IConnectableTCPConnection
    {
        public void Start(EndPoint address, Func<bool, SocketError> connected)
        {
            return;
        }
    }
}
