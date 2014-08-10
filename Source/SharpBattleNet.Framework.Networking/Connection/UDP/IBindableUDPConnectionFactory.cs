namespace SharpBattleNet.Framework.Networking.Connection.UDP
{
    #region Usings
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    #endregion

    public interface IBindableUDPConnectionFactory
    {
        IBindableUDPConnection Create();
    }
}
