namespace SharpBattleNet.Framework.Networking.Utilities.Collections
{
    #region Usings
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Net.Sockets;
    #endregion

    public interface ISocketEventPool : IProducerConsumerCollection<SocketAsyncEventArgs>
    {
    }
}
