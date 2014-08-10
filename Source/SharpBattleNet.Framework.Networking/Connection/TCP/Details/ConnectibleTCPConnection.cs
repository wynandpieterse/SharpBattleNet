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
        private Func<bool, bool> _connectCallback = null;

        public ConnectibleTCPConnection(ISocketEventPool socketEventBag)
            : base(socketEventBag)
        {
            Guard.AgainstNull(socketEventBag);

            _socketEventBag = socketEventBag;

            return;
        }

        private SocketAsyncEventArgs RequestSocketEvent()
        {
            SocketAsyncEventArgs socketEvent = null;

            if (false == _socketEventBag.TryTake(out socketEvent))
            {
                socketEvent = new SocketAsyncEventArgs();
            }

            socketEvent.RemoteEndPoint = _connectionEndPoint;
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
                _logger.Trace("Failed to add socket event back to socket event pool");
            }

            return;
        }

        private void ProcessConnect(SocketAsyncEventArgs socketEvent)
        {
            Guard.AgainstNull(socketEvent);
            Guard.AgainstNull(_connectCallback);
            Guard.AgainstNull(Socket);

            if (SocketError.Success != socketEvent.SocketError)
            {
                _logger.Debug("Failed to connect to {0}", socketEvent.RemoteEndPoint);
                _logger.Trace("Stated reason for failure to connect is {0}", socketEvent.SocketError);

                _connectCallback(false);

                Socket.Close();
            }
            else
            {
                if (false == _connectCallback(true))
                {
                    _logger.Trace("User refusing to connect to {0}", socketEvent.RemoteEndPoint);

                    Socket.Close();
                }
                else
                {
                    _logger.Trace("User accepted connection to {0}", socketEvent.RemoteEndPoint);

                    StartRecieving();
                }
            }

            RecycleSocketEvent(socketEvent);
            return;
        }

        private void HandleConnectEvent(object sender, SocketAsyncEventArgs socketEvent)
        {
            ProcessConnect(socketEvent);

            return;
        }

        #region IConnectableTCPConnection Members

        public void Start(EndPoint address, Func<bool, bool> connected)
        {
            SocketAsyncEventArgs socketEvent = null;

            Guard.AgainstNull(address);
            Guard.AgainstNull(connected);

            _connectCallback = connected;
            _connectionEndPoint = address;

            _logger.Debug("Connecting with TCP socket to {0}", address);

            socketEvent = RequestSocketEvent();

            Socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            Socket.Bind(new IPEndPoint(IPAddress.Any, 0));

            if (false == Socket.ConnectAsync(socketEvent))
            {
                ProcessConnect(socketEvent);
            }

            return;
        }

        #endregion
    }
}
