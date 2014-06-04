namespace SharpBattleNet.Servers.BattleNetServer
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
    using SharpBattleNet.Framework.BattleNetServer;
    using SharpBattleNet.Framework.Networking;

    internal static class Runner
    {
        private static IKernel _injectionKernel = null;
        private static IBattleNetServer _server = null;

        public static void Start(string[] commandArguments)
        {
            _injectionKernel = new StandardKernel(new FrameworkModule("BattleNetServer"), new NetworkModule(), new BattleNetServerModule());
            _server = _injectionKernel.Get<IBattleNetServer>();

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

