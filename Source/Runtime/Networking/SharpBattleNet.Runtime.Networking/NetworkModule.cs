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

namespace SharpBattleNet.Framework.Networking
{
    #region Usings
    using System;
    using Ninject.Modules;
    using Ninject.Extensions.Factory;
    using SharpBattleNet.Framework.External.BufferPool;
    using SharpBattleNet.Framework.Networking.Utilities.Collections;
    using SharpBattleNet.Framework.Networking.Utilities.Collections.Details;
    using SharpBattleNet.Framework.Networking.Connection.TCP;
    using SharpBattleNet.Framework.Networking.Connection.TCP.Details;
    using SharpBattleNet.Framework.Networking.Connection.UDP;
    using SharpBattleNet.Framework.Networking.Connection.UDP.Details;
    using SharpBattleNet.Framework.Networking.Listeners.TCP;
    using SharpBattleNet.Framework.Networking.Listeners.TCP.Details;
    using SharpBattleNet.Framework.Networking.Listeners.UDP;
    using SharpBattleNet.Framework.Networking.Listeners.UDP.Details;
    using SharpBattleNet.Framework.Networking.PacketHandeling;
    using SharpBattleNet.Framework.Networking.PacketHandeling.Details;
    #endregion

    /// <summary>
    /// Ninject module to load all IoC objects for the network framework library.
    /// </summary>
    public sealed class NetworkModule : NinjectModule
    {
        /// <summary>
        /// Creates a pool of bytes that the networking subsystem will use to
        /// receive and send messages for performance reason in regard to
        /// garbage collection.
        /// </summary>
        /// <returns>
        /// The pool object to use for byte buffer allocations
        /// </returns>
        private SocketBufferPool CreateSocketBufferPool()
        {
            return new SocketBufferPool(1 * 1024 * 1024, 64, 8);
        }

        /// <summary>
        /// Binds all utility classes to the container.
        /// </summary>
        private void BindUtilities()
        {
            Bind<ISocketEventPool>().To<SocketEventPool>().InSingletonScope();
            Bind<ISocketBufferPool>().ToConstant<SocketBufferPool>(CreateSocketBufferPool()).InSingletonScope();

            return;
        }

        /// <summary>
        /// Binds all connection classes to the container.
        /// </summary>
        private void BindConnectionFactories()
        {
            Bind<IConnectableTCPConnectionFactory>().ToFactory();
            Bind<IConnectableTCPConnection>().To<ConnectibleTCPConnection>();

            Bind<IListenerTCPConnectionFactory>().ToFactory();
            Bind<IListenerTCPConnection>().To<ListenerTCPConnection>();

            Bind<IBindableUDPConnectionFactory>().ToFactory();
            Bind<IBindableUDPConnection>().To<BindableUDPConnection>();

            return;
        }

        /// <summary>
        /// Binds all the listener classes to the container.
        /// </summary>
        private void BindListeners()
        {
            Bind<ITCPListenerFactory>().ToFactory();
            Bind<ITCPListener>().To<TCPListener>();

            Bind<IUDPListenerFactory>().ToFactory();
            Bind<IUDPListener>().To<UDPListener>();

            return;
        }

        /// <summary>
        /// Binds all the details that handle packet dispatching.
        /// </summary>
        private void BindPacketHandeling()
        {
            Bind<IPacketDispatcherFactory>().ToFactory();
            Bind<IPacketDispatcher>().To<PacketDispatcher>();

            return;
        }

        /// <summary>
        /// Called by Ninject to bind all desired objects.
        /// </summary>
        public override void Load()
        {
            BindUtilities();
            BindConnectionFactories();
            BindListeners();
            BindPacketHandeling();

            return;
        }
    }
}

