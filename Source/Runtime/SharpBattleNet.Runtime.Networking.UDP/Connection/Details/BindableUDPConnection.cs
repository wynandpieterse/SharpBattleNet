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

namespace SharpBattleNet.Runtime.Networking.UDP.Connection.Details
{
    #region Usings
    using System;
    using System.Net;
    using SharpBattleNet.Runtime.Networking.Connection.Details;
    using SharpBattleNet.Runtime.Networking.Utilities.Collections;
    using SharpBattleNet.Runtime.Utilities.Debugging;
    using SharpBattleNet.Runtime.Networking.Connection;
    #endregion

    internal sealed class BindableUDPConnection : UDPConnectionBase, IBindableUDPConnection
    {
        private readonly ISocketEventPool _socketEventBag = null;
        private readonly ISocketBufferPool _socketBufferPool = null;
        private readonly EndPoint _addressToBind = null;
        private readonly IConnectionSink _notificationListener = null;

        public BindableUDPConnection(EndPoint addressToBind, IConnectionSink notificationListener, ISocketEventPool socketEventBag, ISocketBufferPool socketBufferPool)
            : base(notificationListener, socketEventBag, socketBufferPool)
        {
            Guard.AgainstNull(addressToBind);
            Guard.AgainstNull(socketEventBag);
            Guard.AgainstNull(socketBufferPool);

            _socketEventBag = socketEventBag;
            _socketBufferPool = socketBufferPool;
            _addressToBind = addressToBind;
            _notificationListener = notificationListener;

            return;
        }
    }
}
