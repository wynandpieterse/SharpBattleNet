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

namespace SharpBattleNet.Framework.Networking.Connection.Details
{
    using NLog;
    #region Usings
    using SharpBattleNet.Framework.Networking.Utilities.Collections;
    using SharpBattleNet.Framework.Utilities.Debugging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;
    #endregion

    internal abstract class ConnectionBase : IConnection
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly ISocketEventPool _socketEventBag = null;

        protected Socket Socket { get; set; }

        public ConnectionBase(ISocketEventPool socketEventBag)
        {
            Guard.AgainstNull(socketEventBag);

            _socketEventBag = socketEventBag;

            return;
        }

        public void Send(byte[] buffer, long bufferLenght = 0, EndPoint address = null)
        {
            Guard.AgainstNull(Socket);
            Guard.AgainstNull(buffer);

            if (null != address)
            {
                if (0 == bufferLenght)
                {
                    Socket.SendTo(buffer, (int)buffer.LongLength, SocketFlags.None, address);
                }
                else
                {
                    Socket.SendTo(buffer, (int)bufferLenght, SocketFlags.None, address);
                }
            }
            else
            {
                if (0 == bufferLenght)
                {
                    Socket.Send(buffer, (int)buffer.LongLength, SocketFlags.None);
                }
                else
                {
                    Socket.Send(buffer, (int)bufferLenght, SocketFlags.None);
                }
            }

            return;
        }

        protected void StartRecieving()
        {
            Guard.AgainstNull(Socket);

            _logger.Trace("Start receiving on local endpoint {0}", Socket.LocalEndPoint);

            return;
        }
    }
}
