namespace SharpBattleNet.Framework.Networking.Connection.TCP
{
    #region Usings
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Net;
    using System.Net.Sockets;
    #endregion

    public interface IConnectableTCPConnection : ITCPConnection
    {
        void Start(EndPoint address, Func<SocketError, bool> connected);
    }
}
