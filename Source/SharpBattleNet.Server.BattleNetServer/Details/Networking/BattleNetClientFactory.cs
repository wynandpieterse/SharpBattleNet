namespace SharpBattleNet.Server.BattleNetServer.Details.Networking
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Ninject;
    using Ninject.Modules;

    using NLog;

    using SharpBattleNet;
    using SharpBattleNet.Framework;
    using SharpBattleNet.Framework.Networking;

    internal sealed class BattleNetClientFactory : IClientFactory
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public IClient CreateClient()
        {
            return new BattleNetClient();
        }
    }
}

