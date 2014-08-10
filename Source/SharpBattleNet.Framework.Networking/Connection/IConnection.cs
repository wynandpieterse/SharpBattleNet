namespace SharpBattleNet.Framework.Networking.Connection
{
    #region Usings
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Net;
    #endregion

    public interface IConnection
    {
        void Send(byte[] buffer, long bufferLenght = 0, EndPoint address = null);
    }
}
