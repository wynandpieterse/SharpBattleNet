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

namespace SharpBattleNet.Runtime.Networking.Connection
{
    #region Usings
    using System;
    using System.Net;
    #endregion

    /// <summary>
    /// Base connection interface. All connection classes will derive from this one. Provides common functionality that is usefull to both TCP and UDP connection stuff.
    /// </summary>
    public interface IConnection : IDisposable
    {
        /// <summary>
        /// Send the specified buffer to the remote location.
        /// </summary>
        /// <param name="buffer">The buffer to send to the remote location.</param>
        /// <param name="bufferLenght">The amount of data to send from the buffer, starting at 0. If this is 0, the whole buffer will be sent.</param>
        /// <param name="destination">The remote location where the data should be sent to.</param>
        void Send(byte[] buffer, int bufferLenght = 0, EndPoint destination = null);

        /// <summary>
        /// Starts receiving data asynchronously on this socket. Will notify the event sink on another thread when there is new data that was read.
        /// </summary>
        void StartReceiving();
    }
}
