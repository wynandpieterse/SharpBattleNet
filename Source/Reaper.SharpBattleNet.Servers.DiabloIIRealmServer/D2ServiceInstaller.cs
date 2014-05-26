namespace Reaper.SharpBattleNet.Servers.DiabloIIRealmServer
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
    public class D2RSServiceIntaller : Installer
    {
        public D2RSServiceIntaller()
        {
            var processInstaller = new ServiceProcessInstaller();
            var serviceInstaller = new ServiceInstaller();

            processInstaller.Account = ServiceAccount.LocalService;

            serviceInstaller.ServiceName = "D2RSService";
            serviceInstaller.DisplayName = "SharpBattleNet - Diablo II Realm Server";
            serviceInstaller.Description = "Diablo II Realm Server";
            serviceInstaller.StartType = ServiceStartMode.Automatic;

            this.Installers.Add(processInstaller);
            this.Installers.Add(serviceInstaller);

            return;
        }
    }
}
