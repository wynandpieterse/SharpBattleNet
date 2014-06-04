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

    internal sealed class UDPListener : IUDPListener
    {
        private readonly IClientFactory _clientFactory = null;

        public UDPListener(IClientFactory clientFactory)
        {
            _clientFactory = clientFactory;

            return;
        }

        public Task Start(IPAddress adddress, int port)
        {
            throw new NotImplementedException();
        }

        public Task Stop()
        {
            throw new NotImplementedException();
        }
    }
}

