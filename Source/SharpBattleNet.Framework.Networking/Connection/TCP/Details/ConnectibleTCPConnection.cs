using SharpBattleNet.Framework.Networking.Utilities.Collections;
using SharpBattleNet.Framework.Utilities.Debugging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using SharpBattleNet.Framework.Networking.Connection.Details;

namespace SharpBattleNet.Framework.Networking.Connection.TCP.Details
{
    internal sealed class ConnectibleTCPConnection : TCPConnectionBase, IConnectableTCPConnection
    {
        private readonly ISocketEventPool _socketEventBag = null;

        private EndPoint _connectionEndPoint = null;
        private Func<SocketError, bool> _connectCallback = null;

        public ConnectibleTCPConnection(ISocketEventPool socketEventBag)
            : base(socketEventBag)
        {
            Guard.AgainstNull(socketEventBag);

            _socketEventBag = socketEventBag;

            return;
        }

        private void SetupSocketEventForConnect(SocketAsyncEventArgs socketEvent)
        {
            socketEvent.RemoteEndPoint = _connectionEndPoint;
            socketEvent.Completed += HandleConnectEvent;
        }

        private void ReleaseSocketEvent(SocketAsyncEventArgs socketEvent)
        {
            socketEvent.RemoteEndPoint = null;
            socketEvent.Completed -= HandleConnectEvent;

            _socketEventBag.TryAdd(socketEvent);

            return;
        }

        private void ProcessConnect(SocketAsyncEventArgs socketEvent)
        {
            if (false == _connectCallback(socketEvent.SocketError))
            {
                Socket.Close();
            }
            else
            {
                StartRecieving();
            }

            ReleaseSocketEvent(socketEvent);
            return;
        }

        private void HandleConnectEvent(object sender, SocketAsyncEventArgs socketEvent)
        {
            ProcessConnect(socketEvent);

            return;
        }

        public void Start(EndPoint address, Func<SocketError, bool> connected)
        {
            SocketAsyncEventArgs socketEvent = null;

            Guard.AgainstNull(address);
            Guard.AgainstNull(connected);

            _connectCallback = connected;
            _connectionEndPoint = address;

            if (false == _socketEventBag.TryTake(out socketEvent))
            {
                socketEvent = new SocketAsyncEventArgs();
            }

            SetupSocketEventForConnect(socketEvent);

            if (false == Socket.ConnectAsync(socketEvent))
            {
                ProcessConnect(socketEvent);
            }

            return;
        }
    }
}
