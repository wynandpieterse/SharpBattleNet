namespace Reaper.SharpBattleNet.Framework.Networking
{
    using System;
    using System.Reflection;
    using System.Linq;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Net;
    using System.Net.Sockets;

    public interface IClient
    {
        Socket Socket { get; set; }
        ClientMode Mode { get; set; }

        Task Start();
        Task Stop();
    }
}

