using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SharpBattleNet.Framework.Networking.Connection.TCP.Details
{
    internal sealed class ListenerTCPConnection : BaseTCPConnection, IListenerTCPConnection
    {
        public void Start(Socket acceptedSocket)
        {
            return;
        }
    }
}
