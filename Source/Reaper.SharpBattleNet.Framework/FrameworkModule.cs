namespace Reaper.SharpBattleNet.Framework
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

    using Reaper;
    using Reaper.SharpBattleNet;
    using Reaper.SharpBattleNet.Framework;

    public class FrameworkModule : NinjectModule
    {
        private void ConfigureLogging()
        {
            return;
        }

        public override void Load()
        {
            ConfigureLogging();

            return;
        }
    }
}
