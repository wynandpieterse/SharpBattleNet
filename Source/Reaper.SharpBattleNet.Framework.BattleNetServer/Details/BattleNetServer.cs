namespace Reaper.SharpBattleNet.Framework.BattleNetServer.Details
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
    using System.Net.NetworkInformation;

    using Nini;
    using Nini.Config;
    using Nini.Ini;
    using Nini.Util;

    using NLog;

    using Reaper;
    using Reaper.SharpBattleNet;
    using Reaper.SharpBattleNet.Framework;
    using Reaper.SharpBattleNet.Framework.Networking;
    using Reaper.SharpBattleNet.Framework.BattleNetServer;
    using Reaper.SharpBattleNet.Framework.BattleNetServer.Details;
    using Reaper.SharpBattleNet.Framework.BattleNetServer.Details.Networking;

    internal sealed class BattleNetServer : IBattleNetServer
    {
        private readonly IConfigSource _configuration = null;
        private readonly ITCPServerFactory _serverFactory = null;

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly TCPClientFactory _clientFactory = null;
        private List<ITCPServer> _listeners = null;

        public BattleNetServer(IConfigSource configuration, ITCPServerFactory serverFactory)
        {
            _configuration = configuration;
            _serverFactory = serverFactory;

            _listeners = new List<ITCPServer>();
            _clientFactory = new TCPClientFactory();

            return;
        }

        public async Task Start(string[] commandArguments)
        {
            await StartNetworking();

            return;
        }

        private async Task StartNetworking()
        {
            _logger.Info("Starting networking");

            var section = _configuration.Configs["Networking"];
            if(null == section)
            {
                var interfaces = NetworkInterface.GetAllNetworkInterfaces();
                foreach(var adapter in interfaces)
                {
                    foreach(var address in adapter.GetIPProperties().UnicastAddresses)
                    {
                        if(address.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            await StartNetworkListener(address.Address, 6112);
                        }
                    }
                }
            }
            else
            {
                if(true == section.GetBoolean("ListenOnAllInterfaces", true))
                {
                    var interfaces = NetworkInterface.GetAllNetworkInterfaces();
                    foreach(var adapter in interfaces)
                    {
                        foreach(var address in adapter.GetIPProperties().UnicastAddresses)
                        {
                            if(address.Address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                await StartNetworkListener(address.Address, Convert.ToInt16(section.GetInt("ListenPort", 6112)));
                            }
                        }
                    }
                }
                else
                {
                    await StartNetworkListener(IPAddress.Parse(section.Get("ListenAddress", "0.0.0.0")), Convert.ToInt16(section.GetInt("ListenPort", 6112)));
                }
            }

            return;
        }

        private async Task StartNetworkListener(IPAddress address, short port)
        {
            try
            {
                var listener = _serverFactory.CreateServer();

                await listener.Start(_clientFactory, address, port);

                _listeners.Add(listener);
            }
            catch(Exception ex)
            {
                _logger.WarnException(String.Format("Failed to create listener for {0}:{1}", address, port), ex);
            }

            return;
        }

        public async Task Stop()
        {
            foreach(var listener in _listeners)
            {
                await listener.Stop();
            }

            return;
        }
    }
}

