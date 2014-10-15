using SharpBattleNet.Runtime.Utilities.Debugging;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpBattleNet.Runtime.Networking.Connection;
using SharpBattleNet.Runtime.Utilities.BufferPool;
using System.Reflection;
using SharpBattleNet.Runtime.Utilities.Extensions;
using SharpBattleNet.Runtime.Networking.PacketHandeling.Execution;
using SharpBattleNet.Runtime.Networking.PacketHandeling.Serialization;

namespace SharpBattleNet.Runtime.Networking.PacketHandeling.Dispatching.Details
{
    internal sealed class PacketDispatcher : IPacketDispatcher
    {
        private readonly uint _program = 0;
        private readonly IPacketHeaderExecutor _headerExecutor = null;

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

        public void Process(IBuffer buffer)
        {


            return;
        }
    }
}
