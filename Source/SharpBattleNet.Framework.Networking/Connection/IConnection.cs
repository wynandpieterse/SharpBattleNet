using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace SharpBattleNet.Framework.Networking.Connection
{
    public interface IConnection
    {
        void Send(byte[] buffer, long bufferLenght = 0, EndPoint address = null);
    }
}
