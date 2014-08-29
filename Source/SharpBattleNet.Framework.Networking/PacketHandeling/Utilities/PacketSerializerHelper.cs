using SharpBattleNet.Framework.External.BufferPool;
using SharpBattleNet.Framework.Networking.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpBattleNet.Framework.Networking.PacketHandeling.Utilities
{
    public abstract class PacketSerializerHelper<TPacketIn, TPacketOut> : IPacketSerializer
    {
        public void Deserialize(IConnection connection, IPacketIn packet, IBuffer buffer)
        {
            Deserialize(connection, (TPacketIn)packet, buffer);

            return;
        }

        public void Serialize(IConnection connection, IPacketOut packet, IBuffer buffer)
        {
            Serialize(connection, (TPacketOut)packet, buffer);

            return;
        }

        protected abstract void Deserialize(IConnection connection, TPacketIn packet, IBuffer buffer);
        protected abstract void Serialize(IConnection connection, TPacketOut packet, IBuffer buffer);
    }
}
