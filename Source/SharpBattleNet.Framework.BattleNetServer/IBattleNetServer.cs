namespace Reaper.SharpBattleNet.Framework.BattleNetServer
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IBattleNetServer
    {
        Task Start(string[] commandArguments);
        Task Stop();
    }
}

