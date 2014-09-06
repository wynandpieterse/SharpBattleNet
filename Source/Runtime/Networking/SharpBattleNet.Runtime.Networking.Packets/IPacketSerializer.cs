using SharpBattleNet.Framework.External.BufferPool;
using SharpBattleNet.Framework.Networking.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpBattleNet.Framework.Networking.PacketHandeling
{
    public interface IPacketSerializer
    {
        void Serialize(IConnection connection, IPacketOut packet, IBuffer buffer);
        void Deserialize(IConnection connection, IPacketIn packet, IBuffer buffer);
    }
}
