namespace SharpBattleNet.Framework.Networking.Connection.UDP
{
    #region Usings
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Net;
    #endregion

    public interface IBindableUDPConnection : IUDPConnection
    {
        void Bind(EndPoint address);
    }
}
