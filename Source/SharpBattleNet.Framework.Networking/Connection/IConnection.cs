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

namespace SharpBattleNet.Framework.Networking.Connection
{
    #region Usings
    using System;
    using System.Net;
    #endregion

    /// <summary>
    /// Base connection interface. Contains methods to send data to remote
    /// systems. Used by the packet handlers to communicate with remote
    /// systems.
    /// </summary>
    public interface IConnection
    {
        /// <summary>
        /// Sends the specified buffer to the remote address.
        /// </summary>
        /// <param name="buffer">The data to send to the remote address.</param>
        /// <param name="bufferLenght">
        /// The ammount of data to send from the buffer beginning from index 0.
        /// If this value is 0, the ammount will be deduced from the buffer
        /// itself.
        /// </param>
        /// <param name="address">
        /// The remote address to send the buffer to if this is a connectionless
        /// protocol.
        /// </param>
        void Send(byte[] buffer, int bufferLenght = 0, EndPoint destination = null);

        void StartReceiving();
    }
}
