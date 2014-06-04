namespace SharpBattleNet.Servers.DiabloIIRealmServer
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using System.ServiceProcess;

    internal sealed class D2RSService : ServiceBase
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

