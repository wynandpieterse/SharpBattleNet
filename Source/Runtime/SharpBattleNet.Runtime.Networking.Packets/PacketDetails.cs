using SharpBattleNet.Runtime.Networking.PacketHandeling.Execution;
using SharpBattleNet.Runtime.Networking.PacketHandeling.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpBattleNet.Runtime.Networking.PacketHandeling
{
    public sealed class PacketDetails
    {
        public uint Program { get; set; }
        public uint MessageID { get; set; }
        public IPacketSerializer Serializer { get; set; }
        public IPacketExecutor Executor { get; set; }

        public PacketDetails()
        {
            return;
        }
    }
}
