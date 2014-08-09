using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using SharpBattleNet.Framework.Utilities.Debugging;

namespace SharpBattleNet.Framework.Networking.Utilities
{
    public class SocketEventBag : ConcurrentBag<SocketAsyncEventArgs>
    {
    }
}
