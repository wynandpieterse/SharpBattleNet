using SharpBattleNet.Runtime.Networking.Connection;
using SharpBattleNet.Runtime.Networking.PacketHandeling.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpBattleNet.Runtime.Networking.PacketHandeling.Execution
{
    public interface IPacketExecutor
    {
        void Handle(IPacketIn packet, IConnection remote);
    }
}
