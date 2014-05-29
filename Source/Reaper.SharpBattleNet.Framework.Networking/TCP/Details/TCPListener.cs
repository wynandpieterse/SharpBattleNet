namespace Reaper.SharpBattleNet.Framework.Networking.TCP.Details
{
    using System;
    using System.Reflection;
    using System.Linq;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Net;
    using System.Net.Sockets;

    internal sealed class TCPListener : ITCPListener
    {
        private readonly IClientFactory _clientFactory = null;

        private TcpListener _listener = null;

        public TCPListener(IClientFactory clientFactory)
        {
            _clientFactory = clientFactory;

            return;
        }

        public async Task Start(IPAddress address, int port)
        {
            return;
        }

        public async Task Stop()
        {
            return;
        }
    }
}
