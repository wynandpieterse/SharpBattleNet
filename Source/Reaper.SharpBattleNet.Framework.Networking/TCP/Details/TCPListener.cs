namespace Reaper.SharpBattleNet.Framework.Networking.TCP.Details
{
    using System;
    using System.Reflection;
    using System.Linq;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Net;
    using System.Net.Sockets;

    using NLog;

    internal sealed class TCPListener : ITCPListener
    {
        private readonly IClientFactory _clientFactory = null;
        private readonly CancellationTokenSource _cancelToken = null;
        private readonly ConcurrentBag<IClient> _clients = null;

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private TcpListener _listener = null;

        public TCPListener(IClientFactory clientFactory)
        {
            _clientFactory = clientFactory;

            _cancelToken = new CancellationTokenSource();
            _clients = new ConcurrentBag<IClient>();

            return;
        }

        public async Task Start(IPAddress address, int port)
        {
            await Task.Factory.StartNew(() =>
                    {
                        _listener = new TcpListener(address, port);

                        _listener.Start();

                        Task.Factory.StartNew(AcceptConnections);

                        _logger.Info("Started listening for connections on {0}:{1}", address, port);
                    }
                );

            return;
        }

        public async void AcceptConnections()
        {
            while(false == _cancelToken.IsCancellationRequested)
            {
                if(true == _listener.Pending())
                {
                    var clientSocket = await _listener.AcceptSocketAsync();

                    if(null != clientSocket)
                    {
                        var client = _clientFactory.CreateClient();

                        _logger.Debug("Got new connection from {0}", clientSocket.RemoteEndPoint);

                        _clients.Add(client);

                        client.Mode = ClientMode.TCP;
                        client.Socket = clientSocket;

                        await client.Start();
                    }
                }
            }

            return;
        }

        public async Task Stop()
        {
            IClient workingClient = null;

            _cancelToken.Cancel();

            while(false == _clients.IsEmpty)
            {
                while(true == _clients.TryTake(out workingClient))
                {
                    await workingClient.Stop();
                }
            }

            return;
        }
    }
}
