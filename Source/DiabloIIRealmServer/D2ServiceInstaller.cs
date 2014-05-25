namespace Reaper.SharpBattleNet.DiabloIIRealmServer
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

            serviceInstaller.DisplayName = "SharpBattleNet - Diablo II Realm Server";
            serviceInstaller.StartType = ServiceStartMode.Manual;

            serviceInstaller.ServiceName = "D2RSService";

            this.Installers.Add(processInstaller);
            this.Installers.Add(serviceInstaller);

            return;
        }
    }
}
