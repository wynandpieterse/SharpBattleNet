namespace Reaper.SharpBattleNet.Framework.Networking.Details
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Net;
    using System.Net.Sockets;

    using NLog;

    using Reaper;
    using Reaper.SharpBattleNet;
    using Reaper.SharpBattleNet.Framework;
    using Reaper.SharpBattleNet.Framework.Networking;

    internal class TCPServerFactory : ITCPServerFactory
    {
        public ITCPServer CreateServer()
        {
            return new TCPServer();
        }
    }
}

