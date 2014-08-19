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
    using NLog;
    using SharpBattleNet.Framework.Networking.Utilities.Collections;
    using SharpBattleNet.Framework.Utilities.Debugging;
    using SharpBattleNet.Framework.Utilities.Collections;
    #endregion

    /// <summary>
    /// Contains the base logic that will be usefull for all connection derived
    /// protocols.
    /// </summary>
    internal abstract class ConnectionBase : IConnection
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly ISocketEventPool _socketEventBag = null;
        private readonly IBufferPool _bufferPool = null;

        protected Socket Socket { get; set; }

        /// <summary>
        /// Constructs an empty <see cref="ConnectionBase"/> object.
        /// </summary>
        /// <param name="socketEventBag">
        /// Reference to a pool of <see cref="SocketAsyncEventArgs"/>. Mainly
        /// used for performance benefits.
        /// </param>
        public ConnectionBase(ISocketEventPool socketEventBag, IBufferPoolManager bufferPoolManager)
        {
            Guard.AgainstNull(socketEventBag);
            Guard.AgainstNull(bufferPoolManager);

            _socketEventBag = socketEventBag;
            _bufferPool = bufferPoolManager.Get("NetworkTransmission");

            return;
        }

        #region IConnection Methods

        /// <summary>
        /// Sends the specified buffer to the specified destination.
        /// </summary>
        /// <param name="buffer">The data to be sent to the other side.</param>
        /// <param name="bufferLenght">
        /// The ammount of data to send from buffer from index 0. If the value
        /// of this parameter is 0, the length is deducted from the buffer itself.
        /// </param>
        /// <param name="address">The remote endpoint to send the data to.</param>
        public void Send(byte[] buffer, long bufferLenght = 0, EndPoint address = null)
        {
            Guard.AgainstNull(Socket);
            Guard.AgainstNull(buffer);

            // Not using async sends because they cause extra heap allocations and what not.
            // This way we go directly to the Windows kernel and send the stuff. May need
            // to implement buffering down the line up till a ceiling point, but this is
            // working fine now.
            if (null != address)
            {
                if (0 == bufferLenght)
                {
                    Socket.SendTo(buffer, (int)buffer.LongLength, SocketFlags.None, address);
                }
                else
                {
                    Socket.SendTo(buffer, (int)bufferLenght, SocketFlags.None, address);
                }
            }
            else
            {
                if (0 == bufferLenght)
                {
                    Socket.Send(buffer, (int)buffer.LongLength, SocketFlags.None);
                }
                else
                {
                    Socket.Send(buffer, (int)bufferLenght, SocketFlags.None);
                }
            }

            return;
        }

        #endregion

        /// <summary>
        /// Creates an empty <see cref="SocketAsyncEventArgs"/> that can
        /// be used to receive data. Sets the event callback and requests
        /// a buffer from the buffer pool to recieve the data in.
        /// </summary>
        /// <returns>
        /// A <see cref="SocketAsyncEventArgs"/> that can be used for recieve
        /// operations.
        /// </returns>
        private SocketAsyncEventArgs RequestReceiveEvent()
        {
            SocketAsyncEventArgs socketEvent = null;

            if (false == _socketEventBag.TryTake(out socketEvent))
            {
                socketEvent = new SocketAsyncEventArgs();
            }

            // TODO : Fix this with proper buffer allocations going through a buffer
            // pool.
            /*byte[] buffer = new byte[1024];
            socketEvent.SetBuffer(buffer, 0, buffer.Length);
            socketEvent.Completed += HandleReceiveEvent;*/

            ArraySegment<byte> buffer = _bufferPool.Request();
            socketEvent.SetBuffer(buffer.Array, buffer.Offset, buffer.Count);
            socketEvent.Completed += HandleReceiveEvent;
            socketEvent.UserToken = buffer;

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
        private void RecycleReceiveEvent(SocketAsyncEventArgs socketEvent)
        {
            Guard.AgainstNull(socketEvent);

            socketEvent.SetBuffer(null, 0, 0);
            socketEvent.Completed -= HandleReceiveEvent;

            if (false == _socketEventBag.TryAdd(socketEvent))
            {
                _logger.Trace("Failed to insert receive socket event back into event pool");
            }

            return;
        }

        /// <summary>
        /// Handles a recieve operation from the network subsystem. Retrieves
        /// the data recieved and buffers them, then passes the data buffer on
        /// to the packet handler to do its magic.
        /// </summary>
        /// <param name="socketEvent">
        /// Contains operating system specific information about the recieve
        /// event.
        /// </param>
        private void HandleReceive(SocketAsyncEventArgs socketEvent)
        {
            Guard.AgainstNull(socketEvent);

            // A recieve of 0 usually means that the stream was closed.
            if (socketEvent.BytesTransferred == 0)
            {
                if (SocketType.Stream == Socket.SocketType)
                {
                    _logger.Debug("Connection from {0} closed normally", Socket.RemoteEndPoint);
                }
                else if(SocketType.Dgram == Socket.SocketType)
                {
                    _logger.Debug("Connection from {0} closed normally", socketEvent.RemoteEndPoint);
                }

                return;
            }

            // Start recieving immediatly again.
            StartRecieving();

            // TODO : Fix this by buffering the data and handing it of to the packet
            // manager for multiplexing it.
            _logger.Info(Encoding.ASCII.GetString(socketEvent.Buffer, 0, socketEvent.BytesTransferred));

            RecycleReceiveEvent(socketEvent);

            return;
        }

        /// <summary>
        /// Handles an asynchronous network recieve event.
        /// </summary>
        /// <param name="sender">The originator of the event.</param>
        /// <param name="socketEvent">Contains details about the recieve event.</param>
        private void HandleReceiveEvent(object sender, SocketAsyncEventArgs socketEvent)
        {
            HandleReceive(socketEvent);

            return;
        }

        /// <summary>
        /// Starts asynchronously recieving data on the socket. This should be
        /// called after the socket is bound and connected.
        /// </summary>
        protected void StartRecieving()
        {
            SocketAsyncEventArgs socketEvent = null;

            Guard.AgainstNull(Socket);

            // Give me a nice, clean and fresh SAEA object to work with please.
            socketEvent = RequestReceiveEvent();

            try
            {
                // Start handling the various socket types.
                if (SocketType.Stream == Socket.SocketType)
                {
                    if (false == Socket.ReceiveAsync(socketEvent))
                    {
                        HandleReceive(socketEvent);
                    }
                }
                else if (SocketType.Dgram == Socket.SocketType)
                {
                    if (false == Socket.ReceiveFromAsync(socketEvent))
                    {
                        HandleReceive(socketEvent);
                    }
                }
                else
                {
                    throw new InvalidOperationException("The network library currently does not handle sockets other that stream and datagram");
                }
            }
            catch (ObjectDisposedException ex)
            {
                _logger.Trace("Socket object disposed. Returning from receive loop", ex);

                if (null != socketEvent)
                {
                    RecycleReceiveEvent(socketEvent);
                }
            }
            catch (SocketException ex)
            {
                _logger.Debug("Socket exception", ex);

                if (null != socketEvent)
                {
                    RecycleReceiveEvent(socketEvent);
                }
            }

            return;
        }
    }
}
