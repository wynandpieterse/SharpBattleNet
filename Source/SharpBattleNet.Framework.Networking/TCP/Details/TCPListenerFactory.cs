namespace SharpBattleNet.Framework.Networking.TCP.Details
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

    using NLog;

    internal sealed class TCPListenerFactory : ITCPListenerFactory
    {
        private readonly IClientFactory _clientFactory = null;

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public TCPListenerFactory(IClientFactory clientFactory)
        {
            _clientFactory = clientFactory;

            return;
        }

        public ITCPListener Create()
        {
            return new TCPListener(_clientFactory);
        }
    }
}

