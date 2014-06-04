namespace SharpBattleNet.Framework.Networking
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

    using SharpBattleNet;
    using SharpBattleNet.Framework;
    using SharpBattleNet.Framework.Networking;
    using SharpBattleNet.Framework.Networking.Details;
    using SharpBattleNet.Framework.Networking.TCP;
    using SharpBattleNet.Framework.Networking.TCP.Details;
    using SharpBattleNet.Framework.Networking.UDP;
    using SharpBattleNet.Framework.Networking.UDP.Details;

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

