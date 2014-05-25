namespace Reaper.SharpBattleNet.BattleNetServer
{
    using System;
    using System.Linq;
    using System.Text;
    using System.ComponentModel;
    using System.ServiceProcess;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Configuration;
    using System.Configuration.Install;

    [RunInstaller(runInstaller: true)]
    public class BNSServiceIntaller : Installer
    {
        public BNSServiceIntaller()
        {
            var processInstaller = new ServiceProcessInstaller();
            var serviceInstaller = new ServiceInstaller();

            processInstaller.Account = ServiceAccount.LocalService;

            serviceInstaller.ServiceName = "BNSService";
            serviceInstaller.DisplayName = "SharpBattleNet - Master Server";
            serviceInstaller.Description = "Master Server";
            serviceInstaller.StartType = ServiceStartMode.Automatic;

            this.Installers.Add(processInstaller);
            this.Installers.Add(serviceInstaller);

            return;
        }
    }
}
