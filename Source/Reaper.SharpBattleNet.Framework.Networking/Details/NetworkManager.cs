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
    using System.Net.NetworkInformation;

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

        private readonly List<ITCPListener> _tcpListeners = null;
        private readonly List<IUDPListener> _udpListeners = null;

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public NetworkManager(IConfigSource configuration, ITCPListenerFactory tcpFactory, IUDPListenerFactory udpFactory)
        {
            _configuration = configuration;
            _tcpFactory = tcpFactory;
            _udpFactory = udpFactory;

            _tcpListeners = new List<ITCPListener>();
            _udpListeners = new List<IUDPListener>();

            return;
        }

        public Task ScanAssemblyForPacketHandlers(Assembly assembly)
        {
            throw new NotImplementedException();
        }

        public async Task<ITCPListener> CreateTCPListener(IPAddress address, int port)
        {
            var listener = _tcpFactory.Create();

            await listener.Start(address, port);

            return listener;
        }

        public async Task<IUDPListener> CreateUDPListener(IPAddress address, int port)
        {
            var listener = _udpFactory.Create();

            await listener.Start(address, port);

            return listener;
        }

        public async Task StartNetworking()
        {
            var config = _configuration.Configs["Networking"];

            _logger.Debug("Starting networking subsystem");

            if(false == NetworkInterface.GetIsNetworkAvailable())
            {
                _logger.Warn("Networking is not enabled. Will not be able to use networking subsystem.");
            }
            else
            {
                if(null == config)
                {
                    _logger.Warn("No configuration section found that defines networking. Please add this or no network functionality will be available");
                    return;
                }
                else
                {
                    if(true == config.GetBoolean("TCPEnabled", false))
                    {
                        await StartTCPNetwork(config);
                    }

                    if(true == config.GetBoolean("UDPEnabled", false))
                    {
                        await StartUDPNetwork(config);
                    }
                }
            }

            return;
        }

        private async Task StartTCPNetwork(IConfig configurationContainer)
        {
            var listeningPort = configurationContainer.GetInt("TCPListenPort", 6112);

            if(true == configurationContainer.GetBoolean("TCPListenOnAllInterfaces", false))
            {
                var allAdapters = NetworkInterface.GetAllNetworkInterfaces();

                foreach(var adapter in allAdapters)
                {
                    foreach(var address in adapter.GetIPProperties().UnicastAddresses)
                    {
                        try
                        {
                            // Dont really care to bind to IPv6 addresses here, as all BNv1 games uses that anyway, just
                            // here for completeness

                            var listener = await CreateTCPListener(address.Address, listeningPort);
                            _tcpListeners.Add(listener);
                        }
                        catch(SocketException ex)
                        {
                            _logger.WarnException(String.Format("Failed to start listening for TCP connections on {0}:{1}", address.Address, listeningPort), ex);
                        }
                    }
                }
            }
            else
            {
                var ipAddress = configurationContainer.Get("TCPListenAddress", "0.0.0.0");

                try
                {
                    var listener = await CreateTCPListener(IPAddress.Parse(ipAddress), listeningPort);
                    _tcpListeners.Add(listener);
                }
                catch(SocketException ex)
                {
                    _logger.WarnException(String.Format("Failed to start listening for TCP connections on {0}:{1}. No networking will be available for TCP connections.", ipAddress, listeningPort), ex);
                }
            }

            return;
        }

        private async Task StartUDPNetwork(IConfig configurationContainer)
        {
            var listeningPort = configurationContainer.GetInt("UDPListenPort", 6112);

            if(true == configurationContainer.GetBoolean("UDPListenOnAllInterfaces", false))
            {
                var allAdapters = NetworkInterface.GetAllNetworkInterfaces();

                foreach(var adapter in allAdapters)
                {
                    foreach(var address in adapter.GetIPProperties().UnicastAddresses)
                    {
                        try
                        {
                            // Dont really care to bind to IPv6 addresses here, as all BNv1 games uses that anyway, just
                            // here for completeness

                            var listener = await CreateUDPListener(address.Address, listeningPort);
                            _udpListeners.Add(listener);
                        }
                        catch(SocketException ex)
                        {
                            _logger.WarnException(String.Format("Failed to start listening for UDP connections on {0}:{1}", address.Address, listeningPort), ex);
                        }
                    }
                }
            }
            else
            {
                var ipAddress = configurationContainer.Get("UDPListenAddress", "0.0.0.0");

                try
                {
                    var listener = await CreateUDPListener(IPAddress.Parse(ipAddress), listeningPort);
                    _udpListeners.Add(listener);
                }
                catch(SocketException ex)
                {
                    _logger.WarnException(String.Format("Failed to start listening for UDP connections on {0}:{1}. No networking will be available for UDP connections.", ipAddress, listeningPort), ex);
                }
            }

            return;
        }

        public async Task StopNetworking()
        {
            foreach(var listener in _tcpListeners)
            {
                await listener.Stop();
            }

            foreach(var listener in _udpListeners)
            {
                await listener.Stop();
            }

            _tcpListeners.Clear();
            _udpListeners.Clear();

            return;
        }
    }
}

