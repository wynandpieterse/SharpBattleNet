namespace SharpBattleNet.Framework.Networking.Listeners.UDP
{
    #region Usings
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    #endregion

    public interface IUDPListenerFactory
    {
        IUDPListener Create();
    }
}
