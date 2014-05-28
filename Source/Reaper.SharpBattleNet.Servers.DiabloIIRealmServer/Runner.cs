namespace Reaper.SharpBattleNet.Servers.DiabloIIRealmServer
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
    using Reaper.SharpBattleNet.Framework.DiabloIIRealmServer;

    internal static class Runner
    {
        private static IKernel _injectionKernel = null;
        private static IDiabloIIRealmServer _server = null;

        public static void Start(string[] commandArguments)
        {
            _injectionKernel = new StandardKernel(new FrameworkModule("../../../Configuration/DiabloIIRealmServer.ini"), new DiabloIIRealmServerModule());
            _server = _injectionKernel.Get<IDiabloIIRealmServer>();

            _server.Start(commandArguments);

            return;
        }

        public static void Stop()
        {
            _server.Stop();

            return;
        }
    }
}
