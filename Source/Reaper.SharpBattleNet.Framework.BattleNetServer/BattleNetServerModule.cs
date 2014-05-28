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
    using Reaper.SharpBattleNet.Framework.BattleNetServer;
    using Reaper.SharpBattleNet.Framework.BattleNetServer.Details;

    public class BattleNetServerModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IBattleNetServer>().To<BattleNetServer>().InSingletonScope();

            return;
        }
    }
}
