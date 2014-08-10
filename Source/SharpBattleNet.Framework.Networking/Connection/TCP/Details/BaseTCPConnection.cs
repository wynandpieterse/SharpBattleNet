using SharpBattleNet.Framework.Networking.Utilities.Collections;
using SharpBattleNet.Framework.Utilities.Debugging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SharpBattleNet.Framework.Networking.Connection.TCP.Details
{
    internal abstract class BaseTCPConnection : ITCPConnection
    {
        private readonly ISocketBag _socketBag = null;
        private readonly ISocketEventBag _socketEventBag = null;

        private Socket Socket { get; set; }

        public BaseTCPConnection(ISocketBag socketBag, ISocketEventBag socketEventBag)
        {
            Guard.AgainstNull(socketBag);
            Guard.AgainstNull(socketEventBag);

            _socketBag = socketBag;
            _socketEventBag = socketEventBag;

            return;
        }

        public void Disconnect()
        {
            if(null != Socket)
            {
                Socket.Disconnect(true);

                while (false == _socketBag.TryAdd(Socket)) ;

                Socket = null;
            }

            return;
        }

        public void Send(byte[] buffer, long bufferLenght = 0, EndPoint address = null)
        {
            Guard.AgainstNull(Socket);
            Guard.AgainstNull(buffer);
            
            if(null != address)
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
    }
}
