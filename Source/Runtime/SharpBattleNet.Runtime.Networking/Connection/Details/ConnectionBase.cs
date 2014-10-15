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

namespace SharpBattleNet.Runtime.Networking.Connection.Details
{
    #region Usings
    using System;
    using System.Text;
    using System.Net;
    using System.Net.Sockets;

    using SharpBattleNet;
    using SharpBattleNet.Runtime;
    using SharpBattleNet.Runtime.Utilities;
    using SharpBattleNet.Runtime.Utilities.Debugging;
    using SharpBattleNet.Runtime.Utilities.BufferPool;
    using SharpBattleNet.Runtime.Utilities.Logging;
    using SharpBattleNet.Runtime.Networking;
    using SharpBattleNet.Runtime.Networking.Utilities;
    using SharpBattleNet.Runtime.Networking.Utilities.Collections;
    #endregion

    /// <summary>
    /// Base class for all connections. Contains usefull logic like retrieving and recycling receive events, and handeling asynchronous receive operations.
    /// </summary>
    public abstract class ConnectionBase : IConnection
    {
        private readonly ILog _logger = LogProvider.For<ConnectionBase>();
        private readonly ISocketEventPool _socketEventBag = null;
        private readonly ISocketBufferPool _socketBufferPool = null;
        private readonly IConnectionSink _connectionSink = null;

        private bool _disposed = false;

        protected Socket Socket { get; set; }

        /// <summary>
        /// Constructs the connection object. Called by subclasses. Initializes a few variables that are usefull to all of the connection classes.
        /// </summary>
        /// <param name="_connectionSink">The event sink to call when important connection events happen.</param>
        /// <param name="socketEventBag">Async socket event collection for increasing operation performance.</param>
        /// <param name="socketBufferPool">Async socket buffer pool for increasing operation performance.</param>
        public ConnectionBase(IConnectionSink connectionSink, ISocketEventPool socketEventBag, ISocketBufferPool socketBufferPool)
        {
            Guard.AgainstNull(_connectionSink);
            Guard.AgainstNull(socketEventBag);
            Guard.AgainstNull(socketBufferPool);

            _connectionSink = connectionSink;
            _socketEventBag = socketEventBag;
            _socketBufferPool = socketBufferPool;

            return;
        }

        /// <summary>
        /// Sends the specified buffer of data to the endpoint specified in the address field.
        /// </summary>
        /// <param name="buffer">The data to send to the other side.</param>
        /// <param name="bufferLenght">The amount of data to send from the buffer beginning at 0. If this value is 0, the whole buffer is sent.</param>
        /// <param name="address">The remote end-point to send this packet to. If null, uses the end-point specified at connection time.</param>
        public abstract void Send(byte[] buffer, int bufferLenght = 0, EndPoint address = null);

        /// <summary>
        /// Starts asynchronously receiving data on this connection. This will run in the background and notify the user via the event sink.
        /// </summary>
        public abstract void StartReceiving();

        /// <summary>
        /// Requests and empty asynchronous receive event object. Will create a new one if there are non available on the stack.
        /// </summary>
        /// <returns>An empty socket event argument that can be used with socket receive operations.</returns>
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
        /// Recycles a previously requested socket asynchronous event object.
        /// </summary>
        /// <param name="socketEvent">The object to return back to the pool.</param>
        protected void RecycleReceiveEvent(SocketAsyncEventArgs socketEvent)
        {
            Guard.AgainstNull(socketEvent);
            Guard.AgainstNull(socketEvent.UserToken);

            IBuffer buffer = socketEvent.UserToken as IBuffer;

            Guard.AgainstNull(buffer);

            buffer.Dispose();

            socketEvent.BufferList = null;
            socketEvent.Completed -= HandleReceiveEvent;
            socketEvent.RemoteEndPoint = null;

            if (false == _socketEventBag.TryAdd(socketEvent))
            {
                // Don't worry too much if it fails, as the GC will re-collect it when it sees
                // no more references to it.
                _logger.Debug("Failed to release socket event back to event pool.");
            }

            return;
        }

        /// <summary>
        /// Called by the operating system when there is new data available on the socket. This will pass the data back to the user supplied event sink for further
        /// processing there. This is called on another thread, so care should be taken to synchronize access to resources from here on.
        /// </summary>
        /// <param name="socketEvent">Contains all the parameters about the receive operation.</param>
        protected void HandleReceive(SocketAsyncEventArgs socketEvent)
        {
            bool failed = false;
            bool finished = false;
            Exception exception = null;

            Guard.AgainstNull(socketEvent);
            Guard.AgainstNull(socketEvent.RemoteEndPoint);
            Guard.AgainstNull(socketEvent.UserToken);

            try
            {
                // A receive of 0 usually means that the stream was closed.
                if (0 == socketEvent.BytesTransferred)
                {
                    finished = true;
                }
                else
                {
                    // Handle receive inside sink
                    _connectionSink.OnReceive(socketEvent.RemoteEndPoint, socketEvent.UserToken as IBuffer, socketEvent.BytesTransferred);

                    // Start new receive cycle
                    RecycleReceiveEvent(socketEvent);
                    StartReceiving();
                }
            }
            catch(ObjectDisposedException)
            {
                // Socket was probably disposed before
                finished = true;
            }
            catch(Exception ex)
            {
                // Unknown failure
                failed = true;
                exception = ex;
            }

            // Handle exceptional conditions
            if(true == finished)
            {
                _connectionSink.OnFinished();
            }
            else if(true == failed)
            {
                _connectionSink.OnException(exception);
            }

            // Dispose if need be
            if(true == failed || true == finished)
            {
                Dispose();
            }

            return;
        }

        /// <summary>
        /// Called by the operating system when there is data available for processing in the receive queue.
        /// </summary>
        /// <param name="sender">The originator of the event. Null because it came from the operating system.</param>
        /// <param name="socketEvent">Contains all data of the receive event.</param>
        protected void HandleReceiveEvent(object sender, SocketAsyncEventArgs socketEvent)
        {
            Guard.AgainstDispose(_disposed);
            Guard.AgainstNull(socketEvent);

            HandleReceive(socketEvent);

            return;
        }

        /// <summary>
        /// Called by the garbage colllector or the application to dispose all managed and unmanaged resources back to the operating system.
        /// </summary>
        /// <param name="disposing">True when the application called the method, false otherwise.</param>
        protected virtual void Dispose(bool disposing)
        {
            if(false == _disposed)
            {
                if(true == disposing)
                {
                    // Dispose managed resources
                    if(null != Socket)
                    {
                        Socket.Dispose();
                        Socket = null;
                    }
                }

                // Dispose unmanaged resources
            }

            _disposed = true;

            // Call base dispose

            return;
        }

        /// <summary>
        /// Called when the object is to be disposed, so that all resources can be freed.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);

            return;
        }
    }
}
