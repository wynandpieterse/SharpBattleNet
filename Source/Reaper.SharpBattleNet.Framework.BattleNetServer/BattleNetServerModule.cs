namespace Reaper.SharpBattleNet.Framework.BattleNetServer
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

    using Reaper;
    using Reaper.SharpBattleNet;
    using Reaper.SharpBattleNet.Framework;
    using Reaper.SharpBattleNet.Framework.Networking;
    using Reaper.SharpBattleNet.Framework.BattleNetServer;
    using Reaper.SharpBattleNet.Framework.BattleNetServer.Details;
    using Reaper.SharpBattleNet.Framework.BattleNetServer.Details.Networking;

    public sealed class BattleNetServerModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IBattleNetServer>().To<BattleNetServer>().InSingletonScope();
            Bind<IClientFactory>().To<BattleNetClientFactory>().InSingletonScope();

            return;
        }
    }
}

