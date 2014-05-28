namespace Reaper.SharpBattleNet.Servers.BattleNetServer
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Ninject;

    using Reaper;
    using Reaper.SharpBattleNet;
    using Reaper.SharpBattleNet.Framework;
    using Reaper.SharpBattleNet.Framework.BattleNetServer;

    internal static class Runner
    {
        private static IKernel _injectionKernel = null;
        private static IBattleNetServer _server = null;

        public static void Start(string[] commandArguments)
        {
            _injectionKernel = new StandardKernel(new FrameworkModule("../../../Configuration/BattleNetServer.ini") ,new BattleNetServerModule());
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
