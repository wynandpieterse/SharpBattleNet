using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpBattleNet.Framework.Networking.PacketHandeling.Details
{
    internal sealed class PacketDispatcher : IPacketDispatcher
    {
        private readonly uint _program = 0;

        public PacketDispatcher(uint program)
        {
            _program = program;

            return;
        }

        public void Process(ArraySegment<byte> buffer)
        {
            return;
        }
    }
}
