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

namespace SharpBattleNet.Framework.Networking.Listeners.TCP.Details
{
    #region Usings
    using System;
    using System.Net;
    using System.Net.Sockets;
    using NLog;
    using SharpBattleNet.Framework.Networking.Utilities.Collections;
    using SharpBattleNet.Framework.Utilities.Debugging;
    using SharpBattleNet.Framework.Networking.Connection;
    using SharpBattleNet.Framework.Networking.Connection.TCP;
    #endregion

    internal sealed class TCPListener : ITCPListener
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly ISocketEventPool _socketEvents = null;
        private readonly IListenerTCPConnectionFactory _listenerFactory = null;

        private Socket _listener = null;
        private Func<IConnection, bool> _accepted = null;

        public TCPListener(ISocketEventPool socketEvents, IListenerTCPConnectionFactory listenerFactory)
        {
            Guard.AgainstNull(socketEvents);
            Guard.AgainstNull(listenerFactory);

            _socketEvents = socketEvents;
            _listenerFactory = listenerFactory;

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
                _logger.Trace("Failed to add an instance of a socket event back to the socket event pool");
            }

            return;
        }

        private void HandleAccept(SocketAsyncEventArgs socketEvent)
        {
            IListenerTCPConnection connection = null;

            Guard.AgainstNull(socketEvent);

            _logger.Debug("Got new TCP connection from {0}", socketEvent.AcceptSocket.RemoteEndPoint);

            StartAccept();

            if (SocketError.Success != socketEvent.SocketError)
            {
                if (SocketError.ConnectionReset == socketEvent.SocketError)
                {
                    _logger.Trace("Connection reset from {0}. Possible DOS attack", socketEvent.AcceptSocket.RemoteEndPoint);
                }
                else
                {
                    if (null != socketEvent.AcceptSocket)
                    {
                        _logger.Warn("Socket connection from {0} failed.", socketEvent.AcceptSocket.RemoteEndPoint);
                    }

                    _logger.Trace("Socket accept fail reason : {0}", socketEvent.SocketError);
                }
            }
            else
            {
                try
                {
                    connection = _listenerFactory.Create();
                    connection.Start(socketEvent.AcceptSocket);
                    if (false == _accepted(connection))
                    {
                        connection.Disconnect();
                    }
                }
                catch(Exception ex)
                {
                    _logger.DebugException("Failed to create socket connection", ex);

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
                _logger.TraceException("Socket disposed. Normal exception that happens at exit", ex);

                if (null != socketEvent)
                {
                    RecycleSocketEvent(socketEvent);
                }
            }

            return;
        }

        #region ITCPListener Members

        public void Start(EndPoint address, Func<IConnection, bool> accepted)
        {
            Guard.AgainstNull(address);
            Guard.AgainstNull(accepted);

            _accepted = accepted;

            _logger.Info("Started listening for connections on {0}", address);

            _listener = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            _listener.Bind(address);
            _listener.Listen(64);

            StartAccept();
            return;
        }

        #endregion
    }
}
