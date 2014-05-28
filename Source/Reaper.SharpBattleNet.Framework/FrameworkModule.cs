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
    using Ninject.Activation;

    using Nini;
    using Nini.Config;
    using Nini.Ini;
    using Nini.Util;

    using Reaper;
    using Reaper.SharpBattleNet;
    using Reaper.SharpBattleNet.Framework;

    public class FrameworkModule : NinjectModule
    {
        private readonly string _configurationFile = "";

        public FrameworkModule(string configurationFile)
        {
            _configurationFile = configurationFile;

            return;
        }

        private void ConfigureConfiguration()
        {
            Bind<IConfigSource>().ToMethod<IniConfigSource>(context => new IniConfigSource(_configurationFile)).InSingletonScope();

            return;
        }

        private void ConfigureLogging()
        {
            return;
        }

        public override void Load()
        {
            ConfigureConfiguration();
            ConfigureLogging();

            return;
        }
    }
}
