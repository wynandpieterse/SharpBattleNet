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

namespace SharpBattleNet.Runtime.Networking.PacketHandeling.Dispatching
{
    #region Using
    using System;

    using SharpBattleNet;
    using SharpBattleNet.Runtime;
    using SharpBattleNet.Runtime.Networking;
    using SharpBattleNet.Runtime.Networking.Connection;
    using SharpBattleNet.Runtime.Utilities;
    using SharpBattleNet.Runtime.Utilities.BufferPool;
    #endregion

    /// <summary>
    /// Base packet dispatcher interface that is used by network application to handle recieved data from a network connection. It passed these through various
    /// layers until it arives at the packet itself where the user can handle the packet according to the values received.
    /// </summary>
    public interface IPacketDispatcher : IDisposable
    {
        /// <summary>
        /// Initializes the packet dispatcher. This incorporates the connection into the packet dispatcher so that it knows who to pass through to the handlers
        /// when new messages arive.
        /// </summary>
        /// <param name="connection">The connection that owns this packet dispatcher.</param>
        void Initialize(IConnection connection);

        /// <summary>
        /// Processes a new messages that was received on the network stream. If there is enough data available, passes the packet through the process until the
        /// packet is handled inside user code.
        /// </summary>
        /// <param name="recievedBuffer">The data that was received from the network connection. Do not dispose, will be handled by the network subsystem.</param>
        void Process(IBuffer recievedBuffer);
    }
}
