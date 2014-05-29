namespace Reaper.SharpBattleNet.Framework.Networking
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface ITCPServerFactory
    {
        ITCPServer CreateServer();
    }
}
