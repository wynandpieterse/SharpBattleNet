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
    using System.Net.Sockets;
    using NLog;
    using SharpBattleNet.Framework.Networking.Utilities.Collections;
    using SharpBattleNet.Framework.Utilities.Debugging;
    using SharpBattleNet.Framework.Networking.Connection.Details;
    #endregion

    /// <summary>
    /// Provides common TCP connection functionality.
    /// </summary>
    internal abstract class TCPConnectionBase : ConnectionBase, ITCPConnection
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly ISocketEventPool _socketEventBag = null;
        private readonly ISocketBufferPool _socketBufferPool = null;
        private readonly IConnectionNotifications _notificationListener = null;

        /// <summary>
        /// Construct an empty <see cref="TCPConnectionBase"/>. Should
        /// be called by derived classes.
        /// </summary>
        /// <param name="socketEventBag"></param>
        protected TCPConnectionBase(IConnectionNotifications notificationListener, ISocketEventPool socketEventBag, ISocketBufferPool socketBufferPool)
            : base(notificationListener, socketEventBag, socketBufferPool)
        {
            Guard.AgainstNull(socketEventBag);
            Guard.AgainstNull(socketBufferPool);

            _socketEventBag = socketEventBag;
            _socketBufferPool = socketBufferPool;
            _notificationListener = notificationListener;

            return;
        }

        public sealed override void Send(byte[] buffer, long bufferLenght = 0, System.Net.EndPoint address = null)
        {
            if(0 == bufferLenght)
            {
                Socket.Send(buffer, buffer.Length, SocketFlags.None);
            } 
            else
            {
                Socket.Send(buffer, (int)bufferLenght, SocketFlags.None);
            }

            return;
        }

        public sealed override void StartReceiving()
        {
            SocketAsyncEventArgs socketEvent = null;

            Guard.AgainstNull(Socket);

            // Give me a nice, clean and fresh SAEA object to work with please.
            socketEvent = RequestReceiveEvent();

            try
            {
                if (false == Socket.ReceiveAsync(socketEvent))
                {
                    HandleReceive(socketEvent);
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

        /// <summary>
        /// Implements the <see cref="ITCPConnection.Disconnect"/> 
        /// method. Closes the socket and disconnects from the remote
        /// side.
        /// </summary>
        public void Disconnect()
        {
            if(null != Socket)
            {
                try
                {
                    Socket.Disconnect(true);
                }
                catch (Exception)
                {
                    // Don't really care here because system is going to remove all the stuff from itself
                    // in any case.
                }
            }

            return;
        }
    }
}
