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

namespace SharpBattleNet.Runtime.Networking
{
    #region Usings
    using System;
    using Ninject.Modules;
    using Ninject.Extensions.Factory;
    using SharpBattleNet.Runtime.Networking.TCP.Connection;
    using SharpBattleNet.Runtime.Networking.TCP.Connection.Details;
    using SharpBattleNet.Runtime.Networking.TCP.Listener;
    using SharpBattleNet.Runtime.Networking.TCP.Listener.Details;
    #endregion

    public sealed class NetworkTCPModule : NinjectModule
    {
        private void BindConnectionFactories()
        {
            Bind<IConnectableTCPConnectionFactory>().ToFactory();
            Bind<IConnectableTCPConnection>().To<ConnectibleTCPConnection>();

            Bind<IListenerTCPConnectionFactory>().ToFactory();
            Bind<IListenerTCPConnection>().To<ListenerTCPConnection>();

            return;
        }

        private void BindListeners()
        {
            Bind<ITCPListenerFactory>().ToFactory();
            Bind<ITCPListener>().To<TCPListener>();

            return;
        }

        public override void Load()
        {
            BindConnectionFactories();
            BindListeners();

            return;
        }
    }
}

