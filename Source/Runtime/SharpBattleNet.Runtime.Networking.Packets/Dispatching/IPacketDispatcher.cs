using SharpBattleNet.Runtime.Utilities.BufferPool;
using SharpBattleNet.Runtime.Networking.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpBattleNet.Runtime.Networking.PacketHandeling.Dispatching
{
    public interface IPacketDispatcher
    {
        void Initialize(IConnection connection);
        void Process(IBuffer recievedBuffer);
    }
}
