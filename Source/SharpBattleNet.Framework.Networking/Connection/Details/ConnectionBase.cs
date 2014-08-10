namespace SharpBattleNet.Framework.Networking.Connection.Details
{
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

        }
    }
}
