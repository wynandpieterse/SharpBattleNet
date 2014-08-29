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
    #endregion

    /// <summary>
    /// Implements <see cref="IListenerTCPConnection"/> to provide listeners
    /// with a way to communicate with the outside world.
    /// </summary>
    internal sealed class ListenerTCPConnection : TCPConnectionBase, IListenerTCPConnection
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly ISocketEventPool _socketEventBag = null;
        private readonly ISocketBufferPool _socketBufferPool = null;

        /// <summary>
        /// Constructs an empty <see cref="ListenerTCPConnection"/> object.
        /// </summary>
        /// <param name="socketEventBag">
        /// Collection of <see cref="SocketAsyncEventArgs"/> object. Usefull
        /// for performance reasons.
        /// </param>
        public ListenerTCPConnection(ISocketEventPool socketEventBag, ISocketBufferPool socketBufferPool)
            : base(socketEventBag, socketBufferPool)
        {
            Guard.AgainstNull(socketEventBag);
            Guard.AgainstNull(socketBufferPool);

            _socketEventBag = socketEventBag;
            _socketBufferPool = socketBufferPool;

            return;
        }

        #region IListenerTCPConnection Members

        /// <summary>
        /// Called by the listener subsystem to start the client socket
        /// and begin receiving data.
        /// </summary>
        /// <param name="acceptedSocket">
        /// The operating system socket that was accepted by the listener.
        /// </param>
        public void Start(Socket acceptedSocket)
        {
            Guard.AgainstNull(acceptedSocket);

            Socket = acceptedSocket;

            StartReceiving();

            return;
        }

        #endregion
    }
}
