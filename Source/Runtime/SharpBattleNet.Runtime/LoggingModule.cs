using Ninject.Modules;
using SharpBattleNet.Runtime.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpBattleNet.Runtime
{
    internal sealed class LoggingModule : NinjectModule
    {
        private readonly string _applicationName = "";

        public LoggingModule(string applicationName)
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
