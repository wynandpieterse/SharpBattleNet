namespace SharpBattleNet.Framework.Networking.UDP.Details
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

    internal sealed class UDPListenerFactory : IUDPListenerFactory
    {
        private readonly IClientFactory _clientFactory = null;

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public UDPListenerFactory(IClientFactory clientFactory)
        {
            _clientFactory = clientFactory;

            return;
        }

        public IUDPListener Create()
        {
            return new UDPListener(_clientFactory);
        }
    }
}

