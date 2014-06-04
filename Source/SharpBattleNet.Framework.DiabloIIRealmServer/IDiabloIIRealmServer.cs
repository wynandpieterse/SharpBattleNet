namespace SharpBattleNet.Framework.DiabloIIRealmServer
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IDiabloIIRealmServer
    {
        Task Start(string[] commandArguments);
        Task Stop();
    }
}

