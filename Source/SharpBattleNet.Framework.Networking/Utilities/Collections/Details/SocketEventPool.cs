namespace SharpBattleNet.Framework.Networking.Utilities.Collections.Details
{
    #region Usings
    using System;
    using System.Collections.Generic;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Net.Sockets;
    #endregion

    internal sealed class SocketEventPool : ConcurrentBag<SocketAsyncEventArgs>, ISocketEventPool
    {
    }
}
