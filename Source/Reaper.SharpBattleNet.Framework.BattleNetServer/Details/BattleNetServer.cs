namespace Reaper.SharpBattleNet.Framework.BattleNetServer.Details
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
    using Reaper.SharpBattleNet.Framework.BattleNetServer;

    internal class BattleNetServer : IBattleNetServer
    {
        private readonly IConfigSource _configuration = null;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public BattleNetServer(IConfigSource configuration)
        {
            _configuration = configuration;

            return;
        }

        public void Start(string[] commandArguments)
        {
            return;
        }

        public void Stop()
        {
            return;
        }
    }
}
