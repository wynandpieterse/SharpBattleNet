using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpBattleNet.Runtime.Networking.PacketHandeling
{
    public interface IPacket
    {
        PacketDetails Details { get; }
    }
}
