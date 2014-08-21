using SharpBattleNet.Framework.Networking.PacketHandeling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpBattleNet.Server.MasterServer
{
    [Packet]
    public sealed class TestPacket : IPacket
    {
        internal class Serializer : IPacketSerializer
        {

        }

        internal class Executor : IPacketExecutor
        {

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
