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

namespace SharpBattleNet.Server.MasterServer
{
    #region Usings
    using System;
    using System.Net;
    using System.Net.Sockets;
    using NLog;
    using SharpBattleNet.Framework;
    using SharpBattleNet.Framework.Networking.Listeners.TCP;
    using SharpBattleNet.Framework.Networking.Connection;
    using SharpBattleNet.Framework.Networking.Connection.TCP;
    using SharpBattleNet.Framework.Utilities.Collections;
    #endregion

    internal sealed class MasterServerProgram : IProgram
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly ITCPListenerFactory _listenerFactory = null;
        private ITCPListener _listener = null;
        private readonly IConnectableTCPConnectionFactory _connectionFactory = null;
        private IConnectableTCPConnection _connect = null;
        private readonly IBufferPoolManager _bufferPoolManager = null;

        public MasterServerProgram(ITCPListenerFactory listenerFactory, IConnectableTCPConnectionFactory connectionFactory, IBufferPoolManager bufferPoolManager)
        {
            _listenerFactory = listenerFactory;
            _connectionFactory = connectionFactory;
            _bufferPoolManager = bufferPoolManager;
            return;
        }

        private bool Accepted(IConnection connection)
        {
            return true;
        }

        private bool OnConnected(IConnection connection, bool error)
        {
            if(false == error)
            {
                _logger.Info("Failed to connect");
            }
            else
            {
                _logger.Info("Successfull connection");
            }

            return true;
        }

        public void Start()
        {
            _logger.Info("Hello, World");

            _bufferPoolManager.Create("NetworkTransmission", 1024);

            _listener = _listenerFactory.Create();

            _listener.Start(new IPEndPoint(IPAddress.Any, 2048), Accepted);

            _connect = _connectionFactory.Create();

            _connect.Start(new IPEndPoint(IPAddress.Loopback, 2048), OnConnected);

            return;
        }

        public void Stop()
        {
            _logger.Info("Bye, World");
            return;
        }
    }
}
