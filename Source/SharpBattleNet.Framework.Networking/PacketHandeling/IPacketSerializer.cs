using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpBattleNet.Framework.Networking.PacketHandeling
{
    public interface IPacketSerializer
    {
        void Serialize(IPacketOut packet, ArraySegment<byte> buffer);
        void Deserialize(ArraySegment<byte> buffer, IPacketIn packet);
    }
}
