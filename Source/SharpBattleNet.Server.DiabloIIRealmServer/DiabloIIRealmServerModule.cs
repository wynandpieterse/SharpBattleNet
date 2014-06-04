namespace SharpBattleNet.Framework.DiabloIIRealmServer
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
    using SharpBattleNet.Framework.DiabloIIRealmServer;
    using SharpBattleNet.Framework.DiabloIIRealmServer.Details;

    public sealed class DiabloIIRealmServerModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IDiabloIIRealmServer>().To<DiabloIIRealmServer>().InSingletonScope();

            return;
        }
    }
}

