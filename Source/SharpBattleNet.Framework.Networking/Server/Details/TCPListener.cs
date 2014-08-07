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

namespace SharpBattleNet.Framework.Networking.Server.Details
{
    #region Usings
    using System;
    using System.Net;
    using System.Net.Sockets;
    using SharpBattleNet.Framework.Utilities.Debugging;
    #endregion

    internal sealed class TCPListener : ITCPListener
    {
        private Action<Socket> _acceptedCallback = null;

        private Socket _listenSocket = null;

        public TCPListener()
        {
            return;
        }

        private void OnAccepted(object sender, SocketAsyncEventArgs e)
        {
            if (SocketError.Success != e.SocketError)
            {

            }
            else
            {
                _acceptedCallback(e.AcceptSocket);
                e.AcceptSocket = null;

                StartAccepting(e);
            }

            return;
        }

        private void StartAccepting(SocketAsyncEventArgs e)
        {
            while (false == _listenSocket.AcceptAsync(e))
            {
                OnAccepted(_listenSocket, e);
            }

            return;
        }

        #region ITCPListener Methods

        public void Start(IPEndPoint address, Action<Socket> acceptedCallback)
        {
            SocketAsyncEventArgs socketEvents = null;

            Guard.AgainstNull(address);
            Guard.AgainstNull(acceptedCallback);

            _acceptedCallback = acceptedCallback;

            socketEvents = new SocketAsyncEventArgs();
            socketEvents.Completed += OnAccepted;

            if (address.AddressFamily == AddressFamily.InterNetworkV6)
            {
                _listenSocket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
            }
            else if(address.AddressFamily == AddressFamily.InterNetwork)
            {
                _listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            }
            else
            {
                throw new InvalidOperationException("TCPListener only support IPv4 and IPv6 sockets.");
            }

            _listenSocket.Bind(address);
            _listenSocket.Listen(16);

            StartAccepting(socketEvents);

            return;
        }

        #endregion
    }
}
