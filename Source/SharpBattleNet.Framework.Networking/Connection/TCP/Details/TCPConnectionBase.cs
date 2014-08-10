using SharpBattleNet.Framework.Networking.Utilities.Collections;
using SharpBattleNet.Framework.Utilities.Debugging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using SharpBattleNet.Framework.Networking.Connection.Details;

namespace SharpBattleNet.Framework.Networking.Connection.TCP.Details
{
    internal abstract class TCPConnectionBase : ConnectionBase, ITCPConnection
    {
        private readonly ISocketBag _socketBag = null;
        private readonly ISocketEventBag _socketEventBag = null;

        public TCPConnectionBase(ISocketBag socketBag, ISocketEventBag socketEventBag)
            : base(socketBag, socketEventBag)
        {
            Guard.AgainstNull(socketBag);
            Guard.AgainstNull(socketEventBag);

            _socketBag = socketBag;
            _socketEventBag = socketEventBag;

            return;
        }

        public void Disconnect()
        {
            if(null != Socket)
            {
                Socket.Disconnect(true);

                while (false == _socketBag.TryAdd(Socket)) ;

                Socket = null;
            }

            return;
        }
    }
}
