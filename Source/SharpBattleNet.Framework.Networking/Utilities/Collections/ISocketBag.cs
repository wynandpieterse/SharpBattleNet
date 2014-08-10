using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;

namespace SharpBattleNet.Framework.Networking.Utilities.Collections
{
    public interface ISocketBag : IProducerConsumerCollection<SocketAsyncEventArgs>
    {
    }
}
