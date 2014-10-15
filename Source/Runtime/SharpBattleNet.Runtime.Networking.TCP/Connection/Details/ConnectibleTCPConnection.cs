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

namespace SharpBattleNet.Runtime.Networking.TCP.Connection.Details
{
    #region Usings
    using System;
    using System.Net;
    using System.Net.Sockets;
    using SharpBattleNet.Runtime.Networking.Connection.Details;
    using SharpBattleNet.Runtime.Networking.Utilities.Collections;
    using SharpBattleNet.Runtime.Utilities.Debugging;
    using SharpBattleNet.Runtime.Networking.Connection;
    #endregion

    /// <summary>
    /// Concrete implementation of <see cref="IConnectableTCPConnection"/>.
    /// </summary>
    internal sealed class ConnectibleTCPConnection : TCPConnectionBase, IConnectableTCPConnection
    {
        private readonly ISocketEventPool _socketEventBag = null;
        private readonly ISocketBufferPool _socketBufferPool = null;

        private readonly EndPoint _addressToConnect = null;
        private readonly IConnectableTCPConnectionListener _listener = null;
        private readonly IConnectionSink _notificationListener = null;

        /// <summary>
        /// Constructs an empty <see cref="ConnectibleTCPConnection"/>.
        /// </summary>
        /// <param name="socketEventBag">
        /// Pool of <see cref="SocketAsyncEventArgs"/> for performance reasons.
        /// </param>
        public ConnectibleTCPConnection(EndPoint addressToConnect, IConnectableTCPConnectionListener listener, IConnectionSink notificationListener, ISocketEventPool socketEventBag, ISocketBufferPool socketBufferPool)
            : base(notificationListener, socketEventBag, socketBufferPool)
        {
            Guard.AgainstNull(addressToConnect);
            Guard.AgainstNull(listener);
            Guard.AgainstNull(socketEventBag);
            Guard.AgainstNull(socketBufferPool);

            _socketEventBag = socketEventBag;
            _socketBufferPool = socketBufferPool;
            _addressToConnect = addressToConnect;
            _listener = listener;
            _notificationListener = notificationListener;

            Start();

            return;
        }

        /// <summary>
        /// Requests an empty <see cref="SocketAsyncEventArgs"/> for connection
        /// purposes. Clears the <see cref="SocketAsyncEventArgs"/> and set 
        /// the callback and remote endpoint properties.
        /// </summary>
        /// <returns>
        /// An empty <see cref="SocketAsyncEventArgs"/> that is ready to be used
        /// to connect to a remote endpoint asynchronously.
        /// </returns>
        private SocketAsyncEventArgs RequestSocketEvent()
        {
            SocketAsyncEventArgs socketEvent = null;

            if (false == _socketEventBag.TryTake(out socketEvent))
            {
                socketEvent = new SocketAsyncEventArgs();
            }

            socketEvent.RemoteEndPoint = _addressToConnect;
            socketEvent.Completed += HandleConnectEvent;

            return socketEvent;
        }

        /// <summary>
        /// Recycles a <see cref="SocketAsyncEventArgs"/> back into the pool
        /// for later use. Clears the callback and remote endpoint reference.
        /// </summary>
        /// <param name="socketEvent">
        /// The <see cref="SocketAsyncEventArgs"/> that should be returned
        /// to the pool.
        /// </param>
        private void RecycleSocketEvent(SocketAsyncEventArgs socketEvent)
        {
            Guard.AgainstNull(socketEvent);

            socketEvent.RemoteEndPoint = null;
            socketEvent.Completed -= HandleConnectEvent;

            if(false == _socketEventBag.TryAdd(socketEvent))
            {
                // Don't worry too much about object not being returned to pool as
                // the garbage collector will recollect it when it finds no more
                // references to it
            }

            return;
        }

        /// <summary>
        /// Handles the connection event to the remote endpoint. If a connection
        /// is successfull, calls the callback supplied by the user. If the user 
        /// returns true from the callback, the connection is placed in the 
        /// receiving state. The user can return false from the callback, and the
        /// connection attempt will be aborted.
        /// </summary>
        /// <param name="socketEvent">
        /// The <see cref="SocketAsyncEventArgs"/> that contains details about
        /// the connection event from the operating system.
        /// </param>
        private void ProcessConnect(SocketAsyncEventArgs socketEvent)
        {
            Guard.AgainstNull(socketEvent);
            Guard.AgainstNull(_listener);
            Guard.AgainstNull(Socket);

            if (SocketError.Success != socketEvent.SocketError)
            {
                _listener.ConnectionFailed(this, _addressToConnect);

                Socket.Close();
            }
            else
            {
                if (false == _listener.ConnectionSucceeded(this, _addressToConnect))
                {
                    Socket.Close();
                }
                else
                {
                }
            }

            RecycleSocketEvent(socketEvent);
            return;
        }

        /// <summary>
        /// Event that is fired by the operating system network stack when the
        /// connection was handled asynchronously.
        /// </summary>
        /// <param name="sender">The sender of the socket event.</param>
        /// <param name="socketEvent">Contains information about the connect event.</param>
        private void HandleConnectEvent(object sender, SocketAsyncEventArgs socketEvent)
        {
            // Just pass through to the handler function
            ProcessConnect(socketEvent);

            return;
        }

        /// <inheritdoc/>
        private void Start()
        {
            SocketAsyncEventArgs socketEvent = null;

            Guard.AgainstNull(_addressToConnect);
            Guard.AgainstNull(_listener);

            socketEvent = RequestSocketEvent();

            Socket = new Socket(_addressToConnect.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                Socket.Bind(new IPEndPoint(IPAddress.Any, 0));

                if (false == Socket.ConnectAsync(socketEvent))
                {
                    ProcessConnect(socketEvent);
                }
            }
            catch (ObjectDisposedException ex)
            {
                _listener.ConnectionFailed(this, _addressToConnect);

                return;
            }
            catch (SocketException ex)
            {
                _listener.ConnectionFailed(this, _addressToConnect);

                return;
            }

            return;
        }
    }
}
