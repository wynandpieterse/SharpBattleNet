namespace Reaper.SharpBattleNet.Framework.Networking
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Net;
    using System.Net.Sockets;

    public interface ITCPClient
    {
        Socket Socket { get; set; }
        CancellationTokenSource CancelToken { get; set; }

        void Accepted();
    }
}

