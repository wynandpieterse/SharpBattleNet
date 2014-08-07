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
    using SharpBattleNet.Framework.Networking;

    using SharpBattleNet.Server.BattleNetServer;

    internal static class Runner
    {
        private static IKernel _injectionKernel = null;

        public static void Start(string[] commandArguments)
        {
            _injectionKernel = new StandardKernel(new FrameworkModule("BattleNetServer"), new NetworkModule(), new BattleNetServerModule());

            return;
        }

        public static void Stop()
        {
            return;
        }
    }
}

