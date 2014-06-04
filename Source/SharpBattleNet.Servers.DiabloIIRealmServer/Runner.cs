namespace SharpBattleNet.Servers.DiabloIIRealmServer
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Ninject;

    using SharpBattleNet;
    using SharpBattleNet.Framework;
    using SharpBattleNet.Framework.DiabloIIRealmServer;
    using SharpBattleNet.Framework.Networking;

    internal static class Runner
    {
        private static IKernel _injectionKernel = null;
        private static IDiabloIIRealmServer _server = null;

        public static void Start(string[] commandArguments)
        {
            _injectionKernel = new StandardKernel(new FrameworkModule("DiabloIIRealmServer"), new NetworkModule(), new DiabloIIRealmServerModule());
            _server = _injectionKernel.Get<IDiabloIIRealmServer>();

            _server.Start(commandArguments).Wait();

            return;
        }

        public static void Stop()
        {
            _server.Stop().Wait();

            return;
        }
    }
}

