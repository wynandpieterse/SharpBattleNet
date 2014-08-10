using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace SharpBattleNet.Framework.Networking.Connection.UDP
{
    public interface IBindableUDPConnection : IUDPConnection
    {
        void Bind(EndPoint address);
    }
}
