﻿namespace Reaper.SharpBattleNet.Framework.Networking
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

    public interface ITCPServer
    {
        Task Start(ITCPClientFactory clientFactory, IPAddress listenAddress, short listeningPort);
        Task Stop();
    }
}