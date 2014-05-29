namespace Reaper.SharpBattleNet.Framework.Networking.Details
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

    using NLog;

    using Reaper;
    using Reaper.SharpBattleNet;
    using Reaper.SharpBattleNet.Framework;
    using Reaper.SharpBattleNet.Framework.Networking;

    internal class TCPServer : ITCPServer
    {
        private TcpListener _listener = null;
        private ITCPClientFactory _clientFactory = null;
        private CancellationTokenSource _cancelToken = null;
        private Logger _logger = LogManager.GetCurrentClassLogger();

        public Task Start(ITCPClientFactory clientFactory, IPAddress listenAddress, short listeningPort)
        {
            return Task.Factory.StartNew(() =>
                    {
                        _clientFactory = clientFactory;
                        _cancelToken = new CancellationTokenSource();
                        _listener = new TcpListener(listenAddress, Convert.ToInt32(listeningPort));

                        _listener.Start();
                        Task.Factory.StartNew(AcceptConnections);

                        _logger.Info("Started listening for TCP connections on {0}:{1}", listenAddress, listeningPort);
                    }
                );
        }

        public async void AcceptConnections()
        {
            while(false == _cancelToken.IsCancellationRequested)
            {
                if(_listener.Pending())
                {
                    var clientSocket = await _listener.AcceptSocketAsync();

                    if(null != clientSocket)
                    {
                        var client = _clientFactory.CreateClient();

                        _logger.Debug("New TCP client connected from {0}", clientSocket.RemoteEndPoint);

                        client.CancelToken = _cancelToken;
                        client.Socket = clientSocket;

                        await Task.Factory.StartNew(client.Accepted);
                    }
                }
            }

            return;
        }

        public Task Stop()
        {
            return Task.Factory.StartNew(() =>
                    {
                        _cancelToken.Cancel();
                        _listener.Stop();
                    }
                );
        }
    }
}

