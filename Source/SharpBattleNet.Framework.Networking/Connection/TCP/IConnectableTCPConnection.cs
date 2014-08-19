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

namespace SharpBattleNet.Framework.Networking.Connection.TCP
{
    #region Usings
    using System;
    using System.Net;
    using System.Net.Sockets;
    #endregion

    /// <summary>
    /// Contains the contract that clients can use to connect to a TCP server.
    /// </summary>
    public interface IConnectableTCPConnection : ITCPConnection
    {
        /// <summary>
        /// Starts an asynchronous connection to a TCP server. After a connection
        /// has been established, the connected callback is called. The connected
        /// callback specified wheter the connection was successfull or not.
        /// 
        /// After the callback returns true, the connection is handed over to the
        /// packet multiplexer that handles incomming packets for the system.
        /// </summary>
        /// <param name="address">The remote address to connect to</param>
        /// <param name="connected">
        /// Callback that gets called with details about connection success.
        /// </param>
        void Start(EndPoint address, Func<IConnection, bool, bool> connected);
    }
}
