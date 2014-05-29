﻿namespace Reaper.SharpBattleNet.Framework.Networking.UDP.Details
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

    internal sealed class UDPListenerFactory : IUDPListenerFactory
    {
        public IUDPListener Create()
        {
            return new UDPListener();
        }
    }
}
