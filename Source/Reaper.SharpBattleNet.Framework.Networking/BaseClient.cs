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

    using NLog;

    public class BaseClient : IClient
    {
        private readonly byte[] _dataBuffer = null;

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public BaseClient()
        {
            _dataBuffer = new byte[0xFF];

            return;
        }

        public Socket Socket { get; set; }
        public ClientMode Mode { get; set; }

        public async Task Start()
        {
            await Task.Factory.StartNew(() =>
                    {
                        var socketAsyncArgs = new SocketAsyncEventArgs();

                        socketAsyncArgs.SetBuffer(_dataBuffer, 0, _dataBuffer.Length);
                        socketAsyncArgs.Completed += ProcessEvent;

                        Socket.ReceiveAsync(socketAsyncArgs);
                    }
                );

            return;
        }

        private void ProcessEvent(object sender, SocketAsyncEventArgs args)
        {
            int processedBytes = 0;

            try
            {
                processedBytes = args.BytesTransferred;

                if(0 != processedBytes)
                {
                    _logger.Debug("Got packet from {0}", Socket.RemoteEndPoint);

                    Socket.ReceiveAsync(args);
                }
                else
                {
                    CloseConnection();
                }
            }
            catch(Exception ex)
            {
                _logger.WarnException(String.Format("Failed to received data for client {0}", Socket.RemoteEndPoint), ex);
                CloseConnection();
            }

            return;
        }

        public async Task Stop()
        {
            await Task.Factory.StartNew(() =>
                    {
                        CloseConnection();
                    }
                );

            return;
        }

        private void CloseConnection()
        {
            Socket.Close();

            return;
        }
    }
}
