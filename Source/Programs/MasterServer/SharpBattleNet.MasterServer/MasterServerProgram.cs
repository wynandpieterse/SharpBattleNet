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

namespace SharpBattleNet.MasterServer
{
    #region Usings
    using System;
    using System.Net;
    using System.Net.Sockets;
    using SharpBattleNet.Runtime;
    using SharpBattleNet.Runtime.Networking.Connection;
    using SharpBattleNet.Runtime.Networking.Listeners;
    using SharpBattleNet.External.BufferPool;
    using System.Text;
    using SharpBattleNet.Runtime.Networking.TCP.Listener;
    using SharpBattleNet.Runtime.Networking.TCP.Connection;
    #endregion

    internal sealed class MasterServerProgram : IProgram, IListenerAcceptor, IConnectionNotifications, IConnectableTCPConnectionListener
    {
        private readonly ITCPListenerFactory _listenerFactory = null;
        private ITCPListener _listener = null;
        private readonly IConnectableTCPConnectionFactory _connectionFactory = null;
        private IConnectableTCPConnection _connect = null;

        public MasterServerProgram(ITCPListenerFactory listenerFactory, IConnectableTCPConnectionFactory connectionFactory)
        {
            _listenerFactory = listenerFactory;
            _connectionFactory = connectionFactory;

            return;
        }

        public void Start()
        {
            _listener = _listenerFactory.Listen(new IPEndPoint(IPAddress.Any, 6112), this, this);

            _connect = _connectionFactory.Connect(new IPEndPoint(IPAddress.Loopback, 6112), this, this);

            return;
        }

        public void Stop()
        {
            return;
        }

        public bool ShouldAccept(EndPoint remoteEndpoint, IConnection remoteConnection)
        {
            return true;
        }

        public void Accepted(EndPoint remoteEndpoint, IConnection remoteConnection)
        {
            remoteConnection.StartReceiving();

            return;
        }

        public void OnSend(EndPoint remoteAddress, byte[] dataBuffer, int dataSent)
        {
            return;
        }

        public void OnReceive(EndPoint remoteAddress, IBuffer dataBuffer, int bytesReceived)
        {
            byte[] buffer = new byte[1024];
            dataBuffer.CopyTo(buffer, 0, bytesReceived);

            return;
        }

        public void OnFinished()
        {
            return;
        }

        public void OnException(Exception exception)
        {
            return;
        }

        public void ConnectionFailed(IConnectableTCPConnection connection, EndPoint remoteEndpoint)
        {
            return;
        }

        public bool ConnectionSucceeded(IConnectableTCPConnection connection, EndPoint remoteEndpoint)
        {
            connection.StartReceiving();

            return true;
        }
    }
}
