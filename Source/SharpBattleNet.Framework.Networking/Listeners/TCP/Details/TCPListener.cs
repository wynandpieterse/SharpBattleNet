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

            socketEvent.Completed += HandleAcceptEvent;

            return null;
        }

        private void RecycleSocketEvent(SocketAsyncEventArgs socketEvent)
        {
            Guard.AgainstNull(socketEvent);

            socketEvent.Completed -= HandleAcceptEvent;
            _socketEvents.TryAdd(socketEvent);

            return;
        }

        private void HandleAccept(SocketAsyncEventArgs socketEvent)
        {
            IListenerTCPConnection connection = null;

            StartAccept();

            if (socketEvent.SocketError != SocketError.Success)
            {
                _logger.Warn("Socket connection from {0} failed. Stated reason is : {1}", socketEvent.RemoteEndPoint, socketEvent.SocketError);
            }
            else
            {
                connection = _listenerFactory.Create();
                connection.Start(socketEvent.AcceptSocket);
                if (false == _accepted(connection))
                {
                    connection.Disconnect();
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

            socketEvent = RequestSocketEvent();

            if (false == _listener.AcceptAsync(socketEvent))
            {
                HandleAccept(socketEvent);
            }

            return;
        }

        #region ITCPListener Members

        public void Start(EndPoint address, Func<IConnection, bool> accepted)
        {
            Guard.AgainstNull(address);
            Guard.AgainstNull(accepted);

            _accepted = accepted;

            _listener = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            _listener.Bind(address);
            _listener.Listen(64);

            StartAccept();
            return;
        }

        #endregion
    }
}
