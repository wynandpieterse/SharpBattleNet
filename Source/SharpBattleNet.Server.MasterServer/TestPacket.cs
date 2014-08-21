using SharpBattleNet.Framework.Networking.PacketHandeling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpBattleNet.Framework.Networking.Connection;
using SharpBattleNet.Framework.Networking.PacketHandeling.Utilities;

namespace SharpBattleNet.Server.MasterServer
{
    [Packet]
    public sealed class TestPacket : IPacket
    {
        internal class In : IPacketIn
        {

        }

        internal class Out : IPacketOut
        {

        }

        internal class Serializer : PacketSerializerHelper<In, Out>
        {
            protected override void Deserialize(ArraySegment<byte> buffer, In packet)
            {
                return;
            }

            protected override void Serialize(Out packet, ArraySegment<byte> buffer)
            {
                return;
            }
        }

        internal class Executor : PacketExecutorHelper<In>
        {
            protected override void Handle(In packet, IConnection remote)
            {
                return;
            }
        }

        public PacketDetails Details
        {
            get
            {
                PacketDetails details = new PacketDetails();

                details.Program = 0;
                details.MessageID = 1;
                details.Executor = new Executor();
                details.Serializer = new Serializer();

                return details;
            }
        }
    }
}
