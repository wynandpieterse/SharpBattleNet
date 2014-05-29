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

    public abstract class BaseClient : IClient
    {
        private readonly byte[] _dataBuffer = null;

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private byte[] _readBufferBackup = null;
        private byte[] _readBuffer = null;
        private int _readBufferPosition = 0;

        public BaseClient()
        {
            _dataBuffer = new byte[256];
            _readBufferBackup = new byte[512];
            _readBuffer = new byte[512];
            _readBufferPosition = 0;

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

        public abstract int ProcessBuffer(byte[] buffer, int length);

        private void ProcessEvent(object sender, SocketAsyncEventArgs args)
        {
            int processedBytes = 0;
            int handledBytes = 0;

            try
            {
                processedBytes = args.BytesTransferred;

                if(0 != processedBytes)
                {
                    _logger.Debug("Got {0} bytes from {1}", processedBytes, Socket.RemoteEndPoint);

                    Array.Copy(_dataBuffer, 0, _readBuffer, _readBufferPosition, processedBytes);
                    _readBufferPosition += processedBytes;

                    do
                    {
                        handledBytes = ProcessBuffer(_readBuffer, _readBufferPosition);

                        Array.Copy(_readBuffer, 0, _readBufferBackup, 0, _readBuffer.Length);
                        _readBuffer.Initialize();

                        Array.Copy(_readBufferBackup, handledBytes, _readBuffer, 0, _readBufferPosition - handledBytes);

                        _readBufferPosition -= handledBytes;
                    } while(handledBytes < _readBufferPosition);

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
