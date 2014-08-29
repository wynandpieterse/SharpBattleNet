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

    /// <summary>
    /// Implements <see cref="ITCPListener"/>.
    /// </summary>
    internal sealed class TCPListener : ITCPListener
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly ISocketEventPool _socketEvents = null;
        private readonly IListenerTCPConnectionFactory _listenerFactory = null;
        private readonly IListenerAcceptor _acceptor = null;
        private readonly IConnectionNotifications _notificationListener = null;
        private readonly EndPoint _listenEndpoint = null;

        private Socket _listener = null;

        /// <summary>
        /// Constructs an empty <see cref="TCPListener"/>.
        /// </summary>
        /// <param name="socketEvents">
        /// Pool of <see cref="SocketAsyncEventArgs"/> objects. Mainly for
        /// performance reasons.
        /// </param>
        /// <param name="listenerFactory">
        /// Factory that is used to create new TCP listener connections when
        /// a client is accepted.
        /// </param>
        public TCPListener(EndPoint listenEndpoint, IListenerAcceptor acceptor, IConnectionNotifications notificationListener, ISocketEventPool socketEvents, IListenerTCPConnectionFactory listenerFactory)
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

        /// <summary>
        /// Creates an empty <see cref="SocketAsyncEventArgs"/> that is used
        /// to accept new clients. Sets the accept callback and accept socket
        /// properties to required states.
        /// </summary>
        /// <returns>
        /// A <see cref="SocketAsyncEventArgs"/> that can be used to accept
        /// new clients on a TCP listener.
        /// </returns>
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

        /// <summary>
        /// Recycles a <see cref="SocketAsyncEventArgs"/> back into the
        /// pool.
        /// </summary>
        /// <param name="socketEvent">
        /// The <see cref="SocketAsyncEventArgs"/> object to recycle.
        /// </param>
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

        /// <summary>
        /// Handles an accept event from the operating system network subsystem.
        /// Calls the passed callback function to make sure the server wishes to
        /// accept the new client. A return value of true from the callback states
        /// that the client should be accepted while a value of false closes the
        /// connection.
        /// </summary>
        /// <param name="socketEvent">
        /// Contains information from the operating system about accept operation.
        /// </param>
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
                    connection = _listenerFactory.Create(socketEvent.AcceptSocket, _notificationListener);
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
                    _logger.Debug("Failed to create socket connection", ex);

                    socketEvent.AcceptSocket.Close();
                }
            }

            RecycleSocketEvent(socketEvent);
            return;
        }

        /// <summary>
        /// Handles an asynchronous accept operation.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="socketEvent">
        /// Contains operating system network system information about the accept
        /// event.
        /// </param>
        private void HandleAcceptEvent(object sender, SocketAsyncEventArgs socketEvent)
        {
            HandleAccept(socketEvent);

            return;
        }

        /// <summary>
        /// Starts accepting clients on this listener. Will asynchronously call
        /// the provided callback when new clients are connecting.
        /// </summary>
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
                _logger.Trace("Socket disposed. Normal exception that happens at exit", ex);

                if (null != socketEvent)
                {
                    RecycleSocketEvent(socketEvent);
                }
            }

            return;
        }

        #region ITCPListener Members

        /// <summary>
        /// Starts listening for TCP connection on the desired endpoint and let
        /// the calling code know through the callback when a new client has
        /// connected.
        /// </summary>
        /// <param name="address">
        /// The local endpoint to listen for new TCP connections.
        /// </param>
        /// <param name="accepted">
        /// Called by the accept logic when a new client has connection. Should
        /// return true if the client should be accepted or false if the client
        /// should be disconnected.
        /// </param>
        private void Start()
        {
            _logger.Info("Started listening for connections on {0}", _listenEndpoint);

            _listener = new Socket(_listenEndpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                _listener.Bind(_listenEndpoint);
                _listener.Listen(64);
            }
            catch (ObjectDisposedException ex)
            {
                _logger.Debug("Listener socket has been closed before even beginning accept operation", ex);

                return;
            }
            catch (SocketException ex)
            {
                _logger.Warn("Failed to set an operation on the listener socket", ex);

                return;
            }

            StartAccept();
            return;
        }

        #endregion
    }
}
