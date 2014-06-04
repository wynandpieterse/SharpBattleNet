namespace SharpBattleNet.Servers.BattleNetServer
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Reflection;
    using System.ComponentModel;
    using System.ServiceProcess;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Configuration;
    using System.Configuration.Install;

    using SharpBattleNet;
    using SharpBattleNet.Framework;
    using SharpBattleNet.Framework.Extensions;

    [RunInstaller(runInstaller: true)]
    public sealed class BNSServiceIntaller : Installer
    {
        public BNSServiceIntaller()
        {
            var processInstaller = new ServiceProcessInstaller();
            var serviceInstaller = new ServiceInstaller();
            var currentAssembly = Assembly.GetExecutingAssembly();

            processInstaller.Account = ServiceAccount.LocalService;

            serviceInstaller.ServiceName = "BNSService";
            serviceInstaller.DisplayName = String.Format("{0} - {1}", currentAssembly.GetAssemblyTitle(), currentAssembly.GetAssemblyFileVersion());
            serviceInstaller.Description = currentAssembly.GetAssemblyDescription();
            serviceInstaller.StartType = ServiceStartMode.Automatic;

            this.Installers.Add(processInstaller);
            this.Installers.Add(serviceInstaller);

            return;
        }
    }
}

