namespace Reaper.SharpBattleNet.Framework.BattleNetServer.Details.Networking
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

    using Reaper;
    using Reaper.SharpBattleNet;
    using Reaper.SharpBattleNet.Framework;
    using Reaper.SharpBattleNet.Framework.Networking;

    internal class TCPClientFactory : ITCPClientFactory
    {
        public ITCPClient CreateClient()
        {
            return new TCPClient();
        }
    }
}

