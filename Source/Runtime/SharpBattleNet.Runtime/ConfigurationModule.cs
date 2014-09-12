using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpBattleNet.Runtime
{
    internal sealed class ConfigurationModule : NinjectModule
    {
        private readonly string _applicationName = "";

        public ConfigurationModule(string applicationName)
        {
            _applicationName = applicationName;

            return;
        }

        public override void Load()
        {


            return;
        }
    }
}
