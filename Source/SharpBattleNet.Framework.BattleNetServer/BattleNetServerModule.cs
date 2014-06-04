namespace SharpBattleNet.Framework.BattleNetServer
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

    using SharpBattleNet;
    using SharpBattleNet.Framework;
    using SharpBattleNet.Framework.Networking;
    using SharpBattleNet.Framework.BattleNetServer;
    using SharpBattleNet.Framework.BattleNetServer.Details;
    using SharpBattleNet.Framework.BattleNetServer.Details.Networking;

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

