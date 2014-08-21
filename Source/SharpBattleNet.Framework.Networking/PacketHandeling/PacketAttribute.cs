using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpBattleNet.Framework.Networking.PacketHandeling
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class PacketAttribute : Attribute
    {
        public PacketAttribute()
        {
            return;
        }
    }
}
