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

namespace SharpBattleNet.Runtime.Networking.Listeners.Details
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
    using SharpBattleNet.Runtime.Networking.Connection;
    using SharpBattleNet.Runtime.Networking.Utilities;
    using SharpBattleNet.Runtime.Networking.Utilities.Collections;
    #endregion

    /// <summary>
    /// Provides common functionality to all listeners that are provided by the networking library.
    /// </summary>
    public abstract class ListenerBase : IListener
    {
        private readonly ILog _logger = LogProvider.For<ListenerBase>();
        private readonly ISocketEventPool _socketEventBag = null;
        private readonly ISocketBufferPool _socketBufferPool = null;
        private readonly IListenerSink _listenSink = null;
        private readonly IConnectionSink _connectionSink = null;

        private bool _disposed = false;

        /// <summary>
        /// Constructs the base listener class. Consumes a few variables that will be usefull to subclasses like event pools, and listener and connection sinks.
        /// </summary>
        /// <param name="listenSink">The sink to notify about listen events.</param>
        /// <param name="connectionSink">The sink to notify about connection events inside listen created connections.</param>
        /// <param name="socketEventBag">Async socket event bag for increasing operation performance.</param>
        /// <param name="socketBufferPool">Async socket buffer pool for increasing operation performance.</param>
        public ListenerBase(IListenerSink listenSink, IConnectionSink connectionSink, ISocketEventPool socketEventBag, ISocketBufferPool socketBufferPool)
        {
            Guard.AgainstNull(_connectionSink);
            Guard.AgainstNull(socketEventBag);
            Guard.AgainstNull(socketBufferPool);

            _connectionSink = connectionSink;
            _socketEventBag = socketEventBag;
            _socketBufferPool = socketBufferPool;
            _listenSink = listenSink;
            _connectionSink = connectionSink;

            return;
        }

        /// <summary>
        /// Called by the garbage colllector or the application to dispose all managed and unmanaged resources back to the operating system.
        /// </summary>
        /// <param name="disposing">True when the application called the method, false otherwise.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (false == _disposed)
            {
                if (true == disposing)
                {
                    // Dispose managed resources
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
