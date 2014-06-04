namespace SharpBattleNet.Servers.BattleNetServer
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using System.ServiceProcess;

    internal sealed class BNSService : ServiceBase
    {
        protected override void OnStart(string[] args)
        {
            base.OnStart(args);

            Runner.Start(args);

            return;
        }

        protected override void OnStop()
        {
            base.OnStop();

            Runner.Stop();

            return;
        }
    }
}

