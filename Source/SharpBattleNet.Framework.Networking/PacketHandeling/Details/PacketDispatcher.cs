﻿using SharpBattleNet.Framework.Utilities.Debugging;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpBattleNet.Framework.Networking.Connection;
using SharpBattleNet.Framework.External.BufferPool;
using System.Reflection;
using SharpBattleNet.Framework.Utilities.Debugging;
using SharpBattleNet.Framework.Utilities.Extensions;
using NLog;

namespace SharpBattleNet.Framework.Networking.PacketHandeling.Details
{
    internal sealed class PacketDispatcher : IPacketDispatcher
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

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

        private void ProcessType(Type type)
        {
            PacketAttribute attribute = type.GetCustomAttribute<PacketAttribute>();
            if(null != attribute)
            {
                Type interfaceType = type.GetInterface("IPacket");
                if(null == interfaceType)
                {
                    _logger.Warn("Type {0} has a Packet attribute but does not inherit from the packet interface", type);
                }
                else
                {
                    _logger.Debug("Found packet type on {0}", type);
                }
            }

            return;
        }

        private void ProcessAssembly(Assembly assembly)
        {
            _logger.Debug("Processing assembly {0} for packet types", assembly);

            assembly.GetTypes().ForEachAsync(ProcessType);
            return;
        }

        private void ProcessAssemblyEvent(object sender, AssemblyLoadEventArgs e)
        {
            ProcessAssembly(e.LoadedAssembly);
            return;
        }

        public void Initialize(IConnection connection)
        {
            Guard.AgainstNull(connection);

            _connection = connection;
            _state = PacketDispatcherState.Header;

            AppDomain.CurrentDomain.AssemblyLoad += ProcessAssemblyEvent;
            AppDomain.CurrentDomain.GetAssemblies().ForEachAsync(ProcessAssembly);

            return;
        }

        public void Process(IBuffer buffer)
        {


            return;
        }
    }
}