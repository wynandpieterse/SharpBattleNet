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

namespace SharpBattleNet.Runtime.Networking.TCP.Listener.Details
{
    #region Usings
    using System;
    using System.Net;
    using System.Net.Sockets;
    using SharpBattleNet.Runtime.Networking.Utilities.Collections;
    using SharpBattleNet.Runtime.Utilities.Debugging;
    using SharpBattleNet.Runtime.Networking.Connection;
    using SharpBattleNet.Runtime.Networking.TCP.Connection;
    using SharpBattleNet.Runtime.Networking.Listeners;
    #endregion

    internal sealed class TCPListener : ITCPListener
    {
        private readonly ISocketEventPool _socketEvents = null;
        private readonly IListenerTCPConnectionFactory _listenerFactory = null;
        private readonly IListenerSink _acceptor = null;
        private readonly IConnectionSink _notificationListener = null;
        private readonly EndPoint _listenEndpoint = null;

        private Socket _listener = null;

        public TCPListener(EndPoint listenEndpoint, IListenerSink acceptor, IConnectionSink notificationListener, ISocketEventPool socketEvents, IListenerTCPConnectionFactory listenerFactory)
        {
            Guard.AgainstNull(socketEvents);
            Guard.AgainstNull(listenerFactory);

            _socketEvents = socketEvents;
            _listenerFactory = listenerFactory;
            _acceptor = acceptor;
            _notificationListener = notificationListener;
            _listenEndpoint = listenEndpoint;

            Start();

            return;
        }

        private SocketAsyncEventArgs RequestSocketEvent()
        {
            SocketAsyncEventArgs socketEvent = null;

            if (false == _socketEvents.TryTake(out socketEvent))
            {
                socketEvent = new SocketAsyncEventArgs();
            }

            socketEvent.AcceptSocket = null;
            socketEvent.Completed += HandleAcceptEvent;

            return socketEvent;
        }

        private void RecycleSocketEvent(SocketAsyncEventArgs socketEvent)
        {
            Guard.AgainstNull(socketEvent);

            socketEvent.AcceptSocket = null;
            socketEvent.Completed -= HandleAcceptEvent;

            if(false == _socketEvents.TryAdd(socketEvent))
            {
                // Again, we dont really care about these SAEA objects not making
                // it back into the pool, they will be collected by the GC when
                // it sees that it no longer contains any references.
            }

            return;
        }

        private void HandleAccept(SocketAsyncEventArgs socketEvent)
        {
            IListenerTCPConnection connection = null;

            Guard.AgainstNull(socketEvent);

            StartAccept();

            if (SocketError.Success != socketEvent.SocketError)
            {
                if (SocketError.ConnectionReset == socketEvent.SocketError)
                {
                    
                }
                else
                {
                    
                }
            }
            else
            {
                try
                {
                    connection = _listenerFactory.Accepted(socketEvent.AcceptSocket, _notificationListener);
                    if (false == _acceptor.ShouldAccept(socketEvent.AcceptSocket.RemoteEndPoint, connection))
                    {
                        connection.Disconnect();
                    }
                    else
                    {
                        _acceptor.Accepted(socketEvent.AcceptSocket.RemoteEndPoint, connection);
                    }
                }
                catch(Exception ex)
                {
                    socketEvent.AcceptSocket.Close();
                }
            }

            RecycleSocketEvent(socketEvent);
            return;
        }

        private void HandleAcceptEvent(object sender, SocketAsyncEventArgs socketEvent)
        {
            HandleAccept(socketEvent);

            return;
        }

        private void StartAccept()
        {
            SocketAsyncEventArgs socketEvent = null;

            try
            {
                socketEvent = RequestSocketEvent();

                if (false == _listener.AcceptAsync(socketEvent))
                {
                    HandleAccept(socketEvent);
                }
            }
            catch (ObjectDisposedException ex)
            {
                if (null != socketEvent)
                {
                    RecycleSocketEvent(socketEvent);
                }
            }

            return;
        }

        #region ITCPListener Members
        private void Start()
        {
            _listener = new Socket(_listenEndpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                _listener.Bind(_listenEndpoint);
                _listener.Listen(64);
            }
            catch (ObjectDisposedException ex)
            {
                return;
            }
            catch (SocketException ex)
            {
                return;
            }

            StartAccept();
            return;
        }

        #endregion
    }
}
