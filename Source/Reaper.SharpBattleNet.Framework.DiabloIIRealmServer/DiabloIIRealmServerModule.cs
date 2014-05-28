namespace Reaper.SharpBattleNet.Framework.DiabloIIRealmServer
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
    using Reaper.SharpBattleNet.Framework.DiabloIIRealmServer;
    using Reaper.SharpBattleNet.Framework.DiabloIIRealmServer.Details;

    public class DiabloIIRealmServerModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IDiabloIIRealmServer>().To<DiabloIIRealmServer>().InSingletonScope();

            return;
        }
    }
}
