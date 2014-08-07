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
        private readonly long ListenBacklog = 64;

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

            _logger.Warn("Connection from {0} was bad on listener {1}. Stated socket reason is : {2}", socketEvent.AcceptSocket.RemoteEndPoint, _listenSocket.LocalEndPoint, socketEvent.SocketError);

            try
            {
                socketEvent.AcceptSocket.Close();
                _socketEventBag.Add(socketEvent);
            }
            catch
            {
                // We swallow the exception here because this was a bad accept anyway. So if cant close the socket, or add it to the bag, let the GC
                // deal with it and free it before it breaks something else.
            }


            return;
        }

        private void ProcessAccept(SocketAsyncEventArgs socketEvent)
        {
            Guard.AgainstNull(socketEvent);

            _logger.Debug("Got new connection from {0} on listener {1}", socketEvent.AcceptSocket.RemoteEndPoint, _listenSocket.LocalEndPoint);

            StartAccepting();

            if (socketEvent.SocketError != SocketError.Success)
            {
                HandleBadAccept(socketEvent);
                return;
            }
            else
            {
                try
                {
                    _acceptedCallback(socketEvent.AcceptSocket);
                }
                catch (Exception ex)
                {
                    _logger.DebugException(string.Format("Exception raised inside accept callback for listener {0}", _listenSocket.LocalEndPoint), ex);

                    // In case the socket is by some magical chance still open, close it.
                    try
                    {
                        socketEvent.AcceptSocket.Close();
                    }
                    catch
                    {
                        // No need to worry about this, it's gone in anycase.
                    }
                }

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
            SocketAsyncEventArgs socketEvent = new SocketAsyncEventArgs();

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

            try
            {
                willRaiseEvent = _listenSocket.AcceptAsync(socketEvent);
                if (false == willRaiseEvent)
                {
                    ProcessAccept(socketEvent);
                }
            }
            catch (ObjectDisposedException ex)
            {
                _logger.Debug("Object disposed exception. Usually happens when program closes and network loop still runs.", ex);
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
                _listenSocket.Listen((int)ListenBacklog);

                _logger.Debug("Started new TCP listener on {0}", address);

                StartAccepting();
            }

            return;
        }

        #endregion
    }
}
