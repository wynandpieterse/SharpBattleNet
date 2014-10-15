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

namespace SharpBattleNet.Runtime.Networking.PacketHandeling.Dispatching.Details
{
    #region Using
    using System;
    using System.Collections;
    using System.Collections.Concurrent;

    using SharpBattleNet;
    using SharpBattleNet.Runtime;
    using SharpBattleNet.Runtime.Utilities;
    using SharpBattleNet.Runtime.Utilities.BufferPool;
    using SharpBattleNet.Runtime.Utilities.Debugging;
    using SharpBattleNet.Runtime.Networking;
    using SharpBattleNet.Runtime.Networking.Connection;
    using SharpBattleNet.Runtime.Networking.PacketHandeling;
    using SharpBattleNet.Runtime.Networking.PacketHandeling.Execution;
    using SharpBattleNet.Runtime.Networking.PacketHandeling.Serialization;
    #endregion

    internal sealed class PacketDispatcher : IPacketDispatcher
    {
        private readonly uint _program = 0;
        private readonly IPacketHeaderExecutor _headerExecutor = null;

        private bool _disposed = false;
        private IConnection _connection = null;
        private PacketDispatcherState _state = PacketDispatcherState.Uninitialized;
        private ConcurrentDictionary<Tuple<int, int>, IPacketSerializer> _packetSerializers = null;
        private ConcurrentDictionary<Tuple<int, int>, IPacketExecutor> _packetExecutors = null;

        public PacketDispatcher(uint program, IPacketHeaderExecutor headerExecutor)
        {
            Guard.AgainstNull(headerExecutor);

            _program = program;
            _headerExecutor = headerExecutor;

            _packetSerializers = new ConcurrentDictionary<Tuple<int, int>, IPacketSerializer>();
            _packetExecutors = new ConcurrentDictionary<Tuple<int, int>, IPacketExecutor>();

            return;
        }

        public void Initialize(IConnection connection)
        {
            Guard.AgainstNull(connection);

            _connection = connection;
            _state = PacketDispatcherState.Header;

            return;
        }

        public void Process(IBuffer recievedBuffer)
        {
            return;
        }

        private void Dispose(bool disposing)
        {
            if (false == _disposed)
            {
                if (true == disposing)
                {
                    // Dispose managed resources
                }

                // Dispose unmanaged resources
            }

            _disposed = true;

            // Call base dispose

            return;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);

            return;
        }
    }
}
