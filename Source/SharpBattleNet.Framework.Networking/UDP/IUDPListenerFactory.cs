namespace SharpBattleNet.Framework.Networking.UDP
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

    internal interface IUDPListenerFactory
    {
        IUDPListener Create();
    }
}

