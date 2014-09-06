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

namespace SharpBattleNet.Framework.Networking.Connection.Details
{
    #region Usings
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using SharpBattleNet.Framework.Networking.Utilities.Collections;
    using SharpBattleNet.Framework.Utilities.Debugging;
    using SharpBattleNet.Framework.External.BufferPool;
    #endregion

    /// <summary>
    /// Contains the base logic that will be usefull for all connection derived
    /// protocols.
    /// </summary>
    internal abstract class ConnectionBase : IConnection
    {
        private readonly ISocketEventPool _socketEventBag = null;
        private readonly ISocketBufferPool _socketBufferPool = null;
        private readonly IConnectionNotifications _notificationListener = null;

        protected Socket Socket { get; set; }

        /// <summary>
        /// Constructs an empty <see cref="ConnectionBase"/> object.
        /// </summary>
        /// <param name="socketEventBag">
        /// Reference to a pool of <see cref="SocketAsyncEventArgs"/>. Mainly
        /// used for performance benefits.
        /// </param>
        public ConnectionBase(IConnectionNotifications notificationListener, ISocketEventPool socketEventBag, ISocketBufferPool socketBufferPool)
        {
            Guard.AgainstNull(notificationListener);
            Guard.AgainstNull(socketEventBag);
            Guard.AgainstNull(socketBufferPool);

            _notificationListener = notificationListener;
            _socketEventBag = socketEventBag;
            _socketBufferPool = socketBufferPool;

            return;
        }

        /// <summary>
        /// Sends the specified buffer to the specified destination.
        /// </summary>
        /// <param name="buffer">The data to be sent to the other side.</param>
        /// <param name="bufferLenght">
        /// The ammount of data to send from buffer from index 0. If the value
        /// of this parameter is 0, the length is deducted from the buffer itself.
        /// </param>
        /// <param name="address">The remote endpoint to send the data to.</param>
        public virtual void Send(byte[] buffer, int bufferLenght = 0, EndPoint address = null)
        {
            return;
        }

        /// <summary>
        /// Starts asynchronously receiving data on the socket. This should be
        /// called after the socket is bound and connected.
        /// </summary>
        public virtual void StartReceiving()
        {
            return;
        }

        /// <summary>
        /// Creates an empty <see cref="SocketAsyncEventArgs"/> that can
        /// be used to receive data. Sets the event callback and requests
        /// a buffer from the buffer pool to receive the data in.
        /// </summary>
        /// <returns>
        /// A <see cref="SocketAsyncEventArgs"/> that can be used for receive
        /// operations.
        /// </returns>
        protected SocketAsyncEventArgs RequestReceiveEvent()
        {
            SocketAsyncEventArgs socketEvent = null;

            if (false == _socketEventBag.TryTake(out socketEvent))
            {
                socketEvent = new SocketAsyncEventArgs();
            }

            IBuffer buffer = _socketBufferPool.GetBuffer(1024);

            socketEvent.BufferList = buffer.GetSegments();
            socketEvent.UserToken = buffer;
            socketEvent.Completed += HandleReceiveEvent;
            socketEvent.RemoteEndPoint = null;

            return socketEvent;
        }

        /// <summary>
        /// Returns the specified <see cref="SocketAsyncEventArgs"/> back
        /// to the pool. Clears the event callback and sets the buffer
        /// back to null.
        /// </summary>
        /// <param name="socketEvent">
        /// The <see cref="SocketAsyncEventArgs"/> to return to the pool.
        /// </param>
        protected void RecycleReceiveEvent(SocketAsyncEventArgs socketEvent)
        {
            Guard.AgainstNull(socketEvent);
            Guard.AgainstNull(socketEvent.UserToken);

            IBuffer buffer = socketEvent.UserToken as IBuffer;

            buffer.Dispose();

            socketEvent.BufferList = null;
            socketEvent.Completed -= HandleReceiveEvent;
            socketEvent.RemoteEndPoint = null;

            if (false == _socketEventBag.TryAdd(socketEvent))
            {
                // Don't worry too much if it fails, as the GC will re-collect it when it sees
                // no more references to it.
            }

            return;
        }

        /// <summary>
        /// Handles a receive operation from the network subsystem. Retrieves
        /// the data recieved and buffers them, then passes the data buffer on
        /// to the packet handler to do its magic.
        /// </summary>
        /// <param name="socketEvent">
        /// Contains operating system specific information about the receive
        /// event.
        /// </param>
        protected void HandleReceive(SocketAsyncEventArgs socketEvent)
        {
            Guard.AgainstNull(socketEvent);
            Guard.AgainstNull(socketEvent.RemoteEndPoint);
            Guard.AgainstNull(socketEvent.UserToken);

            // A receive of 0 usually means that the stream was closed.
            if (socketEvent.BytesTransferred == 0)
            {
                return;
            }

            _notificationListener.OnReceive(socketEvent.RemoteEndPoint, socketEvent.UserToken as IBuffer, socketEvent.BytesTransferred);

            RecycleReceiveEvent(socketEvent);
            StartReceiving();

            return;
        }

        /// <summary>
        /// Handles an asynchronous network receive event.
        /// </summary>
        /// <param name="sender">The originator of the event.</param>
        /// <param name="socketEvent">Contains details about the receive event.</param>
        protected void HandleReceiveEvent(object sender, SocketAsyncEventArgs socketEvent)
        {
            Guard.AgainstNull(socketEvent);

            HandleReceive(socketEvent);

            return;
        }
    }
}
