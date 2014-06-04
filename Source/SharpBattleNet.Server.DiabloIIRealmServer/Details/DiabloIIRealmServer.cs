namespace SharpBattleNet.Server.DiabloIIRealmServer.Details
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Nini;
    using Nini.Config;
    using Nini.Ini;
    using Nini.Util;

    using NLog;

    using SharpBattleNet;
    using SharpBattleNet.Framework;
    using SharpBattleNet.Framework.Networking;
    using SharpBattleNet.Framework.DiabloIIRealmServer;

    internal sealed class DiabloIIRealmServer : IDiabloIIRealmServer
    {
        private readonly IConfigSource _configuration = null;
        private readonly INetworkManager _networkManager = null;

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public DiabloIIRealmServer(IConfigSource configuration, INetworkManager networkManager)
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

