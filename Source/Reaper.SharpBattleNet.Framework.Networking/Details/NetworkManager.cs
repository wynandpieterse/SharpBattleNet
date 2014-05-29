namespace Reaper.SharpBattleNet.Framework.Networking.Details
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

    using Reaper;
    using Reaper.SharpBattleNet;
    using Reaper.SharpBattleNet.Framework;
    using Reaper.SharpBattleNet.Framework.Networking;
    using Reaper.SharpBattleNet.Framework.Networking.TCP;
    using Reaper.SharpBattleNet.Framework.Networking.UDP;

    internal class NetworkManager : INetworkManager
    {
        public Task ScanAssemblyForPacketHandlers(Assembly assembly)
        {
            throw new NotImplementedException();
        }

        public ITCPListener CreateTCPListener(IPAddress address, int port)
        {
            throw new NotImplementedException();
        }

        public IUDPListener CreateUDPListener(IPAddress address, int port)
        {
            throw new NotImplementedException();
        }

        public Task StartNetworking()
        {
            throw new NotImplementedException();
        }
    }
}
