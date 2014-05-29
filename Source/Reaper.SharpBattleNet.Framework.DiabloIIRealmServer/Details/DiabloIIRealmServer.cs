namespace Reaper.SharpBattleNet.Framework.DiabloIIRealmServer.Details
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

    using Reaper;
    using Reaper.SharpBattleNet;
    using Reaper.SharpBattleNet.Framework;
    using Reaper.SharpBattleNet.Framework.DiabloIIRealmServer;

    internal sealed class DiabloIIRealmServer : IDiabloIIRealmServer
    {
        private readonly IConfigSource _configuration = null;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public DiabloIIRealmServer(IConfigSource configuration)
        {
            _configuration = configuration;

            return;
        }

        public async Task Start(string[] commandArguments)
        {
            return;
        }

        public async Task Stop()
        {
            return;
        }
    }
}

