namespace SharpBattleNet.Framework.Networking.Connection.TCP.Details
{
    #region Usings
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
    #endregion

    internal abstract class TCPConnectionBase : ConnectionBase, ITCPConnection
    {
        private readonly ISocketEventPool _socketEventBag = null;

        public TCPConnectionBase(ISocketEventPool socketEventBag)
            : base(socketEventBag)
        {
            Guard.AgainstNull(socketEventBag);

            _socketEventBag = socketEventBag;

            return;
        }

        public void Disconnect()
        {
            if(null != Socket)
            {
                Socket.Disconnect(true);

                Socket = null;
            }

            return;
        }
    }
}
