namespace Reaper.SharpBattleNet.Framework.Networking
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
    using Ninject.Activation;

    using Reaper;
    using Reaper.SharpBattleNet;
    using Reaper.SharpBattleNet.Framework;
    using Reaper.SharpBattleNet.Framework.Networking;
    using Reaper.SharpBattleNet.Framework.Networking.Details;

    public sealed class NetworkModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ITCPServerFactory>().To<TCPServerFactory>();

            return;
        }
    }
}

