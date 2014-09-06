using SharpBattleNet.Framework.External.BufferPool;
using SharpBattleNet.Framework.Networking.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpBattleNet.Framework.Networking.PacketHandeling
{
    public interface IPacketDispatcher
    {
        void Initialize(IConnection connection);
        void Process(IBuffer recievedBuffer);
    }
}
