using SharpBattleNet.Framework.Utilities.Debugging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpBattleNet.Framework.Networking.Connection;

namespace SharpBattleNet.Framework.Networking.PacketHandeling.Details
{
    internal sealed class PacketDispatcher : IPacketDispatcher
    {
        private readonly uint _program = 0;
        private readonly IPacketHeaderExecutor _headerExecutor = null;

        private IConnection _connection = null;
        private PacketDispatcherState _state = PacketDispatcherState.Uninitialized;

        public PacketDispatcher(uint program, IPacketHeaderExecutor headerExecutor)
        {
            Guard.AgainstNull(headerExecutor);

            _program = program;
            _headerExecutor = headerExecutor;

            return;
        }

        public void Initialize(IConnection connection)
        {
            Guard.AgainstNull(connection);

            _connection = connection;
            _state = PacketDispatcherState.Header;

            return;
        }

        public void Process(ArraySegment<byte> buffer)
        {


            return;
        }
    }
}
