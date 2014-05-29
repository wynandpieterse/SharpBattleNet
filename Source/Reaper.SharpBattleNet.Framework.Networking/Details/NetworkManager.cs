namespace Reaper.SharpBattleNet.Framework.Networking.Details
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

    using Nini;
    using Nini.Config;
    using Nini.Ini;
    using Nini.Util;

    using Reaper;
    using Reaper.SharpBattleNet;
    using Reaper.SharpBattleNet.Framework;
    using Reaper.SharpBattleNet.Framework.Networking;
    using Reaper.SharpBattleNet.Framework.Networking.TCP;
    using Reaper.SharpBattleNet.Framework.Networking.UDP;

    internal class NetworkManager : INetworkManager
    {
        private readonly IConfigSource _configuration = null;
        private readonly ITCPListenerFactory _tcpFactory = null;
        private readonly IUDPListenerFactory _udpFactory = null;

        private readonly CancellationTokenSource _cancelToken = null;

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public NetworkManager(IConfigSource configuration, ITCPListenerFactory tcpFactory, IUDPListenerFactory udpFactory)
        {
            _configuration = configuration;
            _tcpFactory = tcpFactory;
            _udpFactory = udpFactory;

            _cancelToken = new CancellationTokenSource();

            return;
        }

        public Task ScanAssemblyForPacketHandlers(Assembly assembly)
        {
            throw new NotImplementedException();
        }

        public async Task<ITCPListener> CreateTCPListener(IPAddress address, int port)
        {
            throw new NotImplementedException();
        }

        public async Task<IUDPListener> CreateUDPListener(IPAddress address, int port)
        {
            throw new NotImplementedException();
        }

        public Task StartNetworking()
        {
            throw new NotImplementedException();
        }

        public Task StopNetworking()
        {
            throw new NotImplementedException();
        }
    }
}
