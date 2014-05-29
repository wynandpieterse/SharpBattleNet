namespace Reaper.SharpBattleNet.Framework.BattleNetServer.Details.Networking
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using NLog;

    using Reaper;
    using Reaper.SharpBattleNet;
    using Reaper.SharpBattleNet.Framework;
    using Reaper.SharpBattleNet.Framework.Networking;

    internal enum BattleNetClient
    {
        None = 0,
        Game = 1,
        FileTransfer = 2,
        Telnet = 3
    }

    internal sealed class BNClient : BaseClient
    {
        private bool _clientTypeSelected = false;
        private BattleNetClient _clientType = BattleNetClient.None;

        private bool _newPacketRead = false;
        private int _nextPacketID = 0;
        private int _nextPacketLength = 0;

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public override int ProcessBuffer(byte[] buffer, int length)
        {
            if(false == _clientTypeSelected)
            {
                var clientType = buffer[0];
                switch(clientType)
                {
                    case 1:
                        _clientType = BattleNetClient.Game;
                        break;
                    case 2:
                        _clientType = BattleNetClient.FileTransfer;
                        break;
                    case 3:
                        _clientType = BattleNetClient.Telnet;
                        break;
                    default:
                        throw new Exception(String.Format("Unknown client type selected for {0} - {1}", Socket.RemoteEndPoint, clientType));
                }

                _clientTypeSelected = true;
                _logger.Debug("Client {0} selected protocol {1}", Socket.RemoteEndPoint, _clientType);
                return 1;
            }
            else
            {
                if(false == _newPacketRead)
                {
                    _newPacketRead = true;
                    return 1;
                }

                if(_nextPacketID == 0)
                {
                    _nextPacketID = buffer[0];
                    _logger.Debug("Next Packet ID : {0}", _nextPacketID);
                    return 1;
                }

                if(_nextPacketLength == 0)
                {
                    if(length >= 2)
                    {
                        var lengthBuffer = new byte[2];

                        lengthBuffer[0] = buffer[0];
                        lengthBuffer[1] = buffer[1];

                        _nextPacketLength = BitConverter.ToInt16(lengthBuffer, 0);
                        _logger.Debug("Next Packet Lenght : {0}", _nextPacketLength);
                        return 2;
                    }
                    else
                    {
                        return 0;
                    }
                }

                if(length >= (_nextPacketLength - 4))
                {
                    _logger.Debug("YAY");

                    var packetLength = _nextPacketLength - 4;

                    _newPacketRead = false;
                    _nextPacketID = 0;
                    _nextPacketLength = 0;

                    return packetLength;
                }
            }

            return 0;
        }
    }
}
