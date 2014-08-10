﻿namespace SharpBattleNet.Framework.Networking.Connection.TCP
{
    #region Usings
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    #endregion

    public interface IConnectableTCPConnectionFactory
    {
        IConnectableTCPConnection Create();
    }
}
