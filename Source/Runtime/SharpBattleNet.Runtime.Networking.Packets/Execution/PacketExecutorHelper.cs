using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpBattleNet.Framework.Networking.Connection;

namespace SharpBattleNet.Framework.Networking.PacketHandeling.Utilities
{
    public abstract class PacketExecutorHelper<TPacketIn> : IPacketExecutor
    {
        public PacketExecutorHelper()
        {
            return;
        }

        public void Handle(IPacketIn packet, IConnection remote)
        {
            Handle((TPacketIn)packet, remote);

            return;
        }

        protected abstract void Handle(TPacketIn packet, IConnection remote);
    }
}
