#region Header
//
//    _  _   ____        _   _   _         _   _      _   
//  _| || |_| __ )  __ _| |_| |_| | ___   | \ | | ___| |_ 
// |_  .. _ |  _ \ / _` | __| __| |/ _ \  |  \| |/ _ \ __|
// |_      _| |_) | (_| | |_| |_| |  __/_ | |\  |  __/ |_ 
//   |_||_| |____/ \__,_|\__|\__|_|\___(_)_ | \_|\___|\__|
//
// The MIT License
// 
// Copyright(c) 2014 Wynand Pieters. https://github.com/wpieterse/SharpBattleNet

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
#endregion

namespace SharpBattleNet.Runtime.Networking.Connection.Details
{
    #region Usings
    using System;
    using System.Text;
    using System.Net;
    using System.Net.Sockets;

    using SharpBattleNet;
    using SharpBattleNet.Runtime;
    using SharpBattleNet.Runtime.Utilities;
    using SharpBattleNet.Runtime.Utilities.Debugging;
    using SharpBattleNet.Runtime.Utilities.BufferPool;
    using SharpBattleNet.Runtime.Utilities.Logging;
    using SharpBattleNet.Runtime.Networking;
    using SharpBattleNet.Runtime.Networking.Utilities;
    using SharpBattleNet.Runtime.Networking.Utilities.Collections;
    #endregion

    public abstract class ConnectionBase : IConnection
    {
        private readonly ILog _logger = LogProvider.For<ConnectionBase>();
        private readonly ISocketEventPool _socketEventBag = null;
        private readonly ISocketBufferPool _socketBufferPool = null;
        private readonly IConnectionSink _connectionSink = null;

        private bool _disposed = false;

        protected Socket Socket { get; set; }

        public ConnectionBase(IConnectionSink connectionSink, ISocketEventPool socketEventBag, ISocketBufferPool socketBufferPool)
        {
            Guard.AgainstNull(connectionSink);
            Guard.AgainstNull(socketEventBag);
            Guard.AgainstNull(socketBufferPool);

            _connectionSink = connectionSink;
            _socketEventBag = socketEventBag;
            _socketBufferPool = socketBufferPool;

            return;
        }

        public abstract void Send(byte[] buffer, int bufferLenght = 0, EndPoint address = null);
        public abstract void StartReceiving();

        protected SocketAsyncEventArgs RequestReceiveEvent()
        {
            SocketAsyncEventArgs socketEvent = null;

            if (false == _socketEventBag.TryTake(out socketEvent))
            {
                socketEvent = new SocketAsyncEventArgs();
            }

            IBuffer buffer = _socketBufferPool.GetBuffer(1024);

            socketEvent.BufferList = buffer.GetSegments();
            socketEvent.UserToken = buffer;
            socketEvent.Completed += HandleReceiveEvent;
            socketEvent.RemoteEndPoint = null;

            return socketEvent;
        }

        protected void RecycleReceiveEvent(SocketAsyncEventArgs socketEvent)
        {
            Guard.AgainstNull(socketEvent);
            Guard.AgainstNull(socketEvent.UserToken);

            IBuffer buffer = socketEvent.UserToken as IBuffer;

            Guard.AgainstNull(buffer);

            buffer.Dispose();

            socketEvent.BufferList = null;
            socketEvent.Completed -= HandleReceiveEvent;
            socketEvent.RemoteEndPoint = null;

            if (false == _socketEventBag.TryAdd(socketEvent))
            {
                // Don't worry too much if it fails, as the GC will re-collect it when it sees
                // no more references to it.
                _logger.Debug("Failed to release socket event back to event pool.");
            }

            return;
        }

        protected void HandleReceive(SocketAsyncEventArgs socketEvent)
        {
            bool failed = false;
            bool finished = false;
            Exception exception = null;

            Guard.AgainstNull(socketEvent);
            Guard.AgainstNull(socketEvent.RemoteEndPoint);
            Guard.AgainstNull(socketEvent.UserToken);

            try
            {
                // A receive of 0 usually means that the stream was closed.
                if (0 == socketEvent.BytesTransferred)
                {
                    finished = true;
                }
                else
                {
                    // Handle receive inside sink
                    _connectionSink.OnReceive(socketEvent.RemoteEndPoint, socketEvent.UserToken as IBuffer, socketEvent.BytesTransferred);

                    // Start new receive cycle
                    RecycleReceiveEvent(socketEvent);
                    StartReceiving();
                }
            }
            catch(ObjectDisposedException)
            {
                // Socket was probably disposed before
                finished = true;
            }
            catch(Exception ex)
            {
                // Unknown failure
                failed = true;
                exception = ex;
            }

            // Handle exceptional conditions
            if(true == finished)
            {
                _connectionSink.OnFinished();
            }
            else if(true == failed)
            {
                _connectionSink.OnException(exception);
            }

            // Dispose if need be
            if(true == failed || true == finished)
            {
                Dispose();
            }

            return;
        }

        protected void HandleReceiveEvent(object sender, SocketAsyncEventArgs socketEvent)
        {
            Guard.AgainstDispose(_disposed);
            Guard.AgainstNull(socketEvent);

            HandleReceive(socketEvent);

            return;
        }

        protected virtual void Dispose(bool disposing)
        {
            if(false == _disposed)
            {
                if(true == disposing)
                {
                    // Dispose managed resources
                    if(null != Socket)
                    {
                        Socket.Dispose();
                        Socket = null;
                    }
                }

                // Dispose unmanaged resources
            }

            _disposed = true;

            // Call base dispose

            return;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);

            return;
        }
    }
}
