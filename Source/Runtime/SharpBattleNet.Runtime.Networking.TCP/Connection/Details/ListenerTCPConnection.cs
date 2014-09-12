﻿#region Header
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
    using System.Net.Sockets;
    using SharpBattleNet.Runtime.Networking.Utilities.Collections;
    using SharpBattleNet.Runtime.Utilities.Debugging;
    #endregion

    /// <summary>
    /// Implements <see cref="IListenerTCPConnection"/> to provide listeners
    /// with a way to communicate with the outside world.
    /// </summary>
    internal sealed class ListenerTCPConnection : TCPConnectionBase, IListenerTCPConnection
    {
        private readonly ISocketEventPool _socketEventBag = null;
        private readonly ISocketBufferPool _socketBufferPool = null;
        private readonly IConnectionNotifications _notificationListener = null;

        /// <summary>
        /// Constructs an empty <see cref="ListenerTCPConnection"/> object.
        /// </summary>
        /// <param name="socketEventBag">
        /// Collection of <see cref="SocketAsyncEventArgs"/> object. Usefull
        /// for performance reasons.
        /// </param>
        public ListenerTCPConnection(Socket acceptedSocket, IConnectionNotifications notificationListener, ISocketEventPool socketEventBag, ISocketBufferPool socketBufferPool)
            : base(notificationListener, socketEventBag, socketBufferPool)
        {
            Guard.AgainstNull(acceptedSocket);
            Guard.AgainstNull(socketEventBag);
            Guard.AgainstNull(socketBufferPool);

            Socket = acceptedSocket;

            _socketEventBag = socketEventBag;
            _socketBufferPool = socketBufferPool;
            _notificationListener = notificationListener;

            return;
        }
    }
}
