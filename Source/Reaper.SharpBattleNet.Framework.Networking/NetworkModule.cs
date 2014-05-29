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
    using Reaper.SharpBattleNet.Framework.Networking.TCP;
    using Reaper.SharpBattleNet.Framework.Networking.TCP.Details;
    using Reaper.SharpBattleNet.Framework.Networking.UDP;
    using Reaper.SharpBattleNet.Framework.Networking.UDP.Details;

    public sealed class NetworkModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ITCPListenerFactory>().To<TCPListenerFactory>().InSingletonScope();
            Bind<IUDPListenerFactory>().To<UDPListenerFactory>().InSingletonScope();
            Bind<INetworkManager>().To<NetworkManager>().InSingletonScope();

            return;
        }
    }
}

