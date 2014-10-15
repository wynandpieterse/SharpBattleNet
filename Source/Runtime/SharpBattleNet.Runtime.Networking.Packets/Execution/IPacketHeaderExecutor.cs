using SharpBattleNet.Runtime.Utilities.BufferPool;
using SharpBattleNet.Runtime.Networking.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpBattleNet.Runtime.Networking.PacketHandeling.Execution
{
    public interface IPacketHeaderExecutor
    {
        bool Handle(IConnection connection, IBuffer completeBuffer, out uint id, out uint lenght);
    }
}
