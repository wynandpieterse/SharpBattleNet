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

    internal sealed class BattleNetServer : IBattleNetServer
    {
        private readonly IConfigSource _configuration = null;
        private readonly INetworkManager _networkManager = null;

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public BattleNetServer(IConfigSource configuration, INetworkManager networkManager)
        {
            _configuration = configuration;
            _networkManager = networkManager;

            return;
        }

        public async Task Start(string[] commandArguments)
        {
            await _networkManager.StartNetworking();

            return;
        }

        public async Task Stop()
        {
            await _networkManager.StopNetworking();

            return;
        }
    }
}

