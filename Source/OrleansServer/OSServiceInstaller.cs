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
    public class OSServiceIntaller : Installer
    {
        public OSServiceIntaller()
        {
            var processInstaller = new ServiceProcessInstaller();
            var serviceInstaller = new ServiceInstaller();

            processInstaller.Account = ServiceAccount.LocalService;

            serviceInstaller.DisplayName = "SharpBattleNet - Orleans Host Server";
            serviceInstaller.StartType = ServiceStartMode.Manual;

            serviceInstaller.ServiceName = "OSService";

            this.Installers.Add(processInstaller);
            this.Installers.Add(serviceInstaller);

            return;
        }
    }
}
