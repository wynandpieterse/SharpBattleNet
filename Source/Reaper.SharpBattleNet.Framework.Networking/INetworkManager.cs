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

    using Reaper;
    using Reaper.SharpBattleNet;
    using Reaper.SharpBattleNet.Framework;
    using Reaper.SharpBattleNet.Framework.Networking;
    using Reaper.SharpBattleNet.Framework.Networking.TCP;
    using Reaper.SharpBattleNet.Framework.Networking.UDP;

    public interface INetworkManager
    {
        Task ScanAssemblyForPacketHandlers(Assembly assembly);

        Task<ITCPListener> CreateTCPListener(IPAddress address, int port);
        Task<IUDPListener> CreateUDPListener(IPAddress address, int port);

        Task StartNetworking();
        Task StopNetworking();
    }
}

