using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpBattleNet.Framework.Networking.PacketHandeling.Utilities
{
    public abstract class PacketSerializerHelper<TPacketIn, TPacketOut> : IPacketSerializer
    {
        public void Deserialize(ArraySegment<byte> buffer, IPacketIn packet)
        {
            Deserialize(buffer, (TPacketIn)packet);

            return;
        }

        public void Serialize(IPacketOut packet, ArraySegment<byte> buffer)
        {
            Serialize((TPacketOut)packet, buffer);

            return;
        }

        protected abstract void Deserialize(ArraySegment<byte> buffer, TPacketIn packet);
        protected abstract void Serialize(TPacketOut packet, ArraySegment<byte> buffer);
    }
}
