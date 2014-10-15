using SharpBattleNet.Runtime.Utilities.BufferPool;
using SharpBattleNet.Runtime.Networking.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpBattleNet.Runtime.Networking.PacketHandeling.Serialization
{
    public interface IPacketSerializer
    {
        void Serialize(IConnection connection, IPacketOut packet, IBuffer buffer);
        void Deserialize(IConnection connection, IPacketIn packet, IBuffer buffer);
    }
}
