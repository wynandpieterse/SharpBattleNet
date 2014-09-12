using SharpBattleNet.Runtime.Networking.PacketHandeling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpBattleNet.Runtime.Networking.Connection;
using SharpBattleNet.Runtime.Networking.PacketHandeling.Utilities;
using SharpBattleNet.External.BufferPool;

namespace SharpBattleNet.Server.MasterServer
{
    public sealed class HeaderExecutor : IPacketHeaderExecutor
    {
        public bool Handle(IConnection connection, IBuffer dataBuffer, out uint id, out uint lenght)
        {
            id = 0;
            lenght = 0;

            return false;
        }
    }

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
            protected override void Deserialize(IConnection connection, In packet, IBuffer buffer)
            {
                return;
            }

            protected override void Serialize(IConnection connection, Out packet, IBuffer buffer)
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
