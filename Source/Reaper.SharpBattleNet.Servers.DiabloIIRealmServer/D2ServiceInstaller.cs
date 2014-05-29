namespace Reaper.SharpBattleNet.Servers.DiabloIIRealmServer
{
    using System;
    using System.Reflection;
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

    using Reaper;
    using Reaper.SharpBattleNet;
    using Reaper.SharpBattleNet.Framework;
    using Reaper.SharpBattleNet.Framework.Extensions;

    [RunInstaller(runInstaller: true)]
    public class D2RSServiceIntaller : Installer
    {
        public D2RSServiceIntaller()
        {
            var processInstaller = new ServiceProcessInstaller();
            var serviceInstaller = new ServiceInstaller();
            var currentAssembly = Assembly.GetExecutingAssembly();

            processInstaller.Account = ServiceAccount.LocalService;

            serviceInstaller.ServiceName = "D2RSService";
            serviceInstaller.DisplayName = String.Format("{0} - {1}", currentAssembly.GetAssemblyTitle(), currentAssembly.GetAssemblyFileVersion());
            serviceInstaller.Description = currentAssembly.GetAssemblyDescription();
            serviceInstaller.StartType = ServiceStartMode.Automatic;

            this.Installers.Add(processInstaller);
            this.Installers.Add(serviceInstaller);

            return;
        }
    }
}
