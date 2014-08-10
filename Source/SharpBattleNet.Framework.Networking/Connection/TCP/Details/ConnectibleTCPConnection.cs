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

namespace SharpBattleNet.Framework.Networking.Connection.TCP.Details
{
    #region Usings
    using System;
    using System.Net;
    using System.Net.Sockets;
    using NLog;
    using SharpBattleNet.Framework.Networking.Connection.Details;
    using SharpBattleNet.Framework.Networking.Utilities.Collections;
    using SharpBattleNet.Framework.Utilities.Debugging;
    #endregion

    internal sealed class ConnectibleTCPConnection : TCPConnectionBase, IConnectableTCPConnection
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly ISocketEventPool _socketEventBag = null;

        private EndPoint _connectionEndPoint = null;
        private Func<SocketError, bool> _connectCallback = null;

        public ConnectibleTCPConnection(ISocketEventPool socketEventBag)
            : base(socketEventBag)
        {
            Guard.AgainstNull(socketEventBag);

            _socketEventBag = socketEventBag;

            return;
        }

        private void SetupSocketEventForConnect(SocketAsyncEventArgs socketEvent)
        {
            socketEvent.RemoteEndPoint = _connectionEndPoint;
            socketEvent.Completed += HandleConnectEvent;

            return;
        }

        private void ReleaseSocketEvent(SocketAsyncEventArgs socketEvent)
        {
            socketEvent.RemoteEndPoint = null;
            socketEvent.Completed -= HandleConnectEvent;

            _socketEventBag.TryAdd(socketEvent);

            return;
        }

        private void ProcessConnect(SocketAsyncEventArgs socketEvent)
        {
            if (false == _connectCallback(socketEvent.SocketError))
            {
                Socket.Close();
            }
            else
            {
                StartRecieving();
            }

            ReleaseSocketEvent(socketEvent);
            return;
        }

        private void HandleConnectEvent(object sender, SocketAsyncEventArgs socketEvent)
        {
            ProcessConnect(socketEvent);

            return;
        }

        public void Start(EndPoint address, Func<SocketError, bool> connected)
        {
            SocketAsyncEventArgs socketEvent = null;

            Guard.AgainstNull(address);
            Guard.AgainstNull(connected);

            _connectCallback = connected;
            _connectionEndPoint = address;

            _logger.Debug("Connecting with TCP socket to {0}", address);

            if (false == _socketEventBag.TryTake(out socketEvent))
            {
                socketEvent = new SocketAsyncEventArgs();
            }

            SetupSocketEventForConnect(socketEvent);

            Socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            Socket.Bind(new IPEndPoint(IPAddress.Any, 0));

            if (false == Socket.ConnectAsync(socketEvent))
            {
                ProcessConnect(socketEvent);
            }

            return;
        }
    }
}
