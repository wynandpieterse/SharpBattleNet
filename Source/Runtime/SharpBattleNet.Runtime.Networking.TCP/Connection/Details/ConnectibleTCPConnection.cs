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

namespace SharpBattleNet.Runtime.Networking.TCP.Connection.Details
{
    #region Usings
    using System;
    using System.Net;
    using System.Net.Sockets;
    using SharpBattleNet.Runtime.Networking.Connection.Details;
    using SharpBattleNet.Runtime.Networking.Utilities.Collections;
    using SharpBattleNet.Runtime.Utilities.Debugging;
    using SharpBattleNet.Runtime.Networking.Connection;
    #endregion

    internal sealed class ConnectibleTCPConnection : TCPConnectionBase, IConnectableTCPConnection
    {
        private readonly ISocketEventPool _socketEventBag = null;
        private readonly ISocketBufferPool _socketBufferPool = null;

        private readonly EndPoint _addressToConnect = null;
        private readonly IConnectableTCPConnectionListener _listener = null;
        private readonly IConnectionSink _notificationListener = null;

        public ConnectibleTCPConnection(EndPoint addressToConnect, IConnectableTCPConnectionListener listener, IConnectionSink notificationListener, ISocketEventPool socketEventBag, ISocketBufferPool socketBufferPool)
            : base(notificationListener, socketEventBag, socketBufferPool)
        {
            Guard.AgainstNull(addressToConnect);
            Guard.AgainstNull(listener);
            Guard.AgainstNull(socketEventBag);
            Guard.AgainstNull(socketBufferPool);

            _socketEventBag = socketEventBag;
            _socketBufferPool = socketBufferPool;
            _addressToConnect = addressToConnect;
            _listener = listener;
            _notificationListener = notificationListener;

            Start();

            return;
        }

        private SocketAsyncEventArgs RequestSocketEvent()
        {
            SocketAsyncEventArgs socketEvent = null;

            if (false == _socketEventBag.TryTake(out socketEvent))
            {
                socketEvent = new SocketAsyncEventArgs();
            }

            socketEvent.RemoteEndPoint = _addressToConnect;
            socketEvent.Completed += HandleConnectEvent;

            return socketEvent;
        }

        private void RecycleSocketEvent(SocketAsyncEventArgs socketEvent)
        {
            Guard.AgainstNull(socketEvent);

            socketEvent.RemoteEndPoint = null;
            socketEvent.Completed -= HandleConnectEvent;

            if(false == _socketEventBag.TryAdd(socketEvent))
            {
                // Don't worry too much about object not being returned to pool as
                // the garbage collector will recollect it when it finds no more
                // references to it
            }

            return;
        }

        private void ProcessConnect(SocketAsyncEventArgs socketEvent)
        {
            Guard.AgainstNull(socketEvent);
            Guard.AgainstNull(_listener);
            Guard.AgainstNull(Socket);

            if (SocketError.Success != socketEvent.SocketError)
            {
                _listener.ConnectionFailed(this, _addressToConnect);

                Socket.Close();
            }
            else
            {
                if (false == _listener.ConnectionSucceeded(this, _addressToConnect))
                {
                    Socket.Close();
                }
                else
                {
                }
            }

            RecycleSocketEvent(socketEvent);
            return;
        }

        private void HandleConnectEvent(object sender, SocketAsyncEventArgs socketEvent)
        {
            // Just pass through to the handler function
            ProcessConnect(socketEvent);

            return;
        }

        private void Start()
        {
            SocketAsyncEventArgs socketEvent = null;

            Guard.AgainstNull(_addressToConnect);
            Guard.AgainstNull(_listener);

            socketEvent = RequestSocketEvent();

            Socket = new Socket(_addressToConnect.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                Socket.Bind(new IPEndPoint(IPAddress.Any, 0));

                if (false == Socket.ConnectAsync(socketEvent))
                {
                    ProcessConnect(socketEvent);
                }
            }
            catch (ObjectDisposedException ex)
            {
                _listener.ConnectionFailed(this, _addressToConnect);

                return;
            }
            catch (SocketException ex)
            {
                _listener.ConnectionFailed(this, _addressToConnect);

                return;
            }

            return;
        }
    }
}
