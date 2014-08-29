using SharpBattleNet.Framework.External.BufferPool;
using SharpBattleNet.Framework.Networking.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpBattleNet.Framework.Networking.PacketHandeling
{
    public interface IPacketHeaderExecutor
    {
        bool Handle(IConnection connection, IBuffer completeBuffer, out uint id, out uint lenght);
    }
}
