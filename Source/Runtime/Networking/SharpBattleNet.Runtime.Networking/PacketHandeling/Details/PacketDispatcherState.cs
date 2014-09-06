using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpBattleNet.Framework.Networking.PacketHandeling.Details
{
    public enum PacketDispatcherState : byte
    {
        Uninitialized = 0,
        Header = 1,
        Data = 2
    }
}
