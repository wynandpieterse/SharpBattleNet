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

namespace SharpBattleNet.Framework.Networking.Server.Details
{
    #region Usings
    using System;
    using System.Net;
    using System.Net.Sockets;
    using NLog;
    using SharpBattleNet.Framework.Utilities.Debugging;
    using SharpBattleNet.Framework.Networking.Utilities;
    #endregion

    internal sealed class TCPListener : ITCPListener
    {
        private Action<Socket> _acceptedCallback = null;

        private Socket _listenSocket = null;
        private SocketEventBag _socketEventBag = null;

        private Logger _logger = LogManager.GetCurrentClassLogger();

        public TCPListener()
        {
            _socketEventBag = new SocketEventBag();

            return;
        }

        private void HandleBadAccept(SocketAsyncEventArgs socketEvent)
        {
            Guard.AgainstNull(socketEvent);

            _logger.Warn("Connection from {0} was bad. Stated socket reason is : {1}", socketEvent.AcceptSocket.RemoteEndPoint, socketEvent.SocketError);

            socketEvent.AcceptSocket.Close();

            _socketEventBag.Add(socketEvent);

            return;
        }

        private void ProcessAccept(SocketAsyncEventArgs socketEvent)
        {
            Guard.AgainstNull(socketEvent);

            _logger.Debug("Got new connection from {0}", socketEvent.AcceptSocket.RemoteEndPoint);

            StartAccepting();

            if (socketEvent.SocketError != SocketError.Success)
            {
                HandleBadAccept(socketEvent);
                return;
            }
            else
            {
                _acceptedCallback(socketEvent.AcceptSocket);

                socketEvent.AcceptSocket = null;
                _socketEventBag.Add(socketEvent);
            }

            return;
        }

        private void AsynchronousAccept(object sender, SocketAsyncEventArgs socketEvent)
        {
            Guard.AgainstNull(socketEvent);

            ProcessAccept(socketEvent);

            return;
        }

        private SocketAsyncEventArgs ConstructAcceptOperation()
        {
            SocketAsyncEventArgs socketEvent = null;

            socketEvent.Completed += AsynchronousAccept;

            return socketEvent;
        }

        private void StartAccepting()
        {
            SocketAsyncEventArgs socketEvent = null;
            bool willRaiseEvent = false;

            if (false == _socketEventBag.TryTake(out socketEvent))
            {
                socketEvent = ConstructAcceptOperation();
            }

            willRaiseEvent = _listenSocket.AcceptAsync(socketEvent);
            if (false == willRaiseEvent)
            {
                ProcessAccept(socketEvent);
            }

            return;
        }

        #region ITCPListener Methods

        public void Start(IPEndPoint address, Action<Socket> acceptedCallback)
        {
            Guard.AgainstNull(address);
            Guard.AgainstNull(acceptedCallback);

            if (address.AddressFamily != AddressFamily.InterNetwork || address.AddressFamily != AddressFamily.InterNetworkV6)
            {
                throw new InvalidOperationException("TCP listener currently only support IPv4 and IPv6 sockets.");
            }
            else
            {
                _listenSocket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                _listenSocket.Bind(address);
                _listenSocket.Listen(64);

                _logger.Debug("Started new TCP listener on {0}", address);

                StartAccepting();
            }

            return;
        }

        #endregion
    }
}
