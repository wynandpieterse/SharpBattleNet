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

    using SharpBattleNet;
    using SharpBattleNet.Runtime;
    using SharpBattleNet.Runtime.Utilities;
    using SharpBattleNet.Runtime.Utilities.BufferPool;
    #endregion

    /// <summary>
    /// User should subclass this interface to receive notifications about whats happening inside a connection class. These methods will be called from the thread pool
    /// when new notifications arive, so you should be carefull to handle synchronization.
    /// </summary>
    public interface IConnectionSink
    {
        /// <summary>
        /// Called when data was sent from a connection to another end-point.
        /// </summary>
        /// <param name="remoteAddress">The address to where the data was sent.</param>
        /// <param name="dataBuffer">The data that was sent to the remote endpoint.</param>
        /// <param name="dataSent">The total amount of data that the operating system managed to send.</param>
        void OnSend(EndPoint remoteAddress, byte[] dataBuffer, int dataSent);

        /// <summary>
        /// Called when new data was received by the operating system, and ready to process by the application.
        /// </summary>
        /// <param name="remoteAddress">The remote address from where the data was receieved.</param>
        /// <param name="dataBuffer">The buffer containing the data that was received.</param>
        /// <param name="dataReceived">The amount of bytes received in this operation.</param>
        void OnReceive(EndPoint remoteAddress, IBuffer dataBuffer, int dataReceived);

        /// <summary>
        /// Called when the connection was closed normally.
        /// </summary>
        void OnFinished();

        /// <summary>
        /// Called when an exception condition was encountered in one of the connection operation. See the exception parameter for more details.
        /// </summary>
        /// <param name="exception">The exception that was passed to this sink, containing more details about the problem.</param>
        void OnException(Exception exception);
    }
}
