namespace Reaper.SharpBattleNet.Framework.BattleNetServer.Details.Networking
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

    internal class TCPClient : ITCPClient
    {
        private byte[] _dataBuffer = null;
        private Logger _logger = LogManager.GetCurrentClassLogger();

        public TCPClient()
        {
            _dataBuffer = new byte[128];

            return;
        }

        public Socket Socket { get; set; }
        public CancellationTokenSource CancelToken { get; set; }

        public void Accepted()
        {
            var socketAsyncArgs = new SocketAsyncEventArgs();

            socketAsyncArgs.SetBuffer(_dataBuffer, 0, _dataBuffer.Length);
            socketAsyncArgs.Completed += Process;

            if(false == CancelToken.IsCancellationRequested)
            {
                Socket.ReceiveAsync(socketAsyncArgs);
            }

            return;
        }

        private void Process(object sender, SocketAsyncEventArgs args)
        {
            try
            {
                if(true == CancelToken.IsCancellationRequested)
                {
                    return;
                }
                else
                {
                    var receivedBytes = args.BytesTransferred;

                    if(0 != receivedBytes)
                    {
                        // Process the packet here
                        _logger.Info("Got packet from {0} - {1}", Socket.RemoteEndPoint, _dataBuffer.Aggregate("", (current, b) => current + " " + b.ToString("X2")));

                        Socket.ReceiveAsync(args);
                    }
                    else
                    {
                        Socket.Close();
                    }
                }
            }
            catch(Exception ex)
            {
                _logger.WarnException("Failed during receive", ex);
                return;
            }

            return;
        }
    }
}

