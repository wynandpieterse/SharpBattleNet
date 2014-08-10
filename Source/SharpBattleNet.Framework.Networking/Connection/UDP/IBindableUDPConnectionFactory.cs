﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpBattleNet.Framework.Networking.Connection.UDP
{
    public interface IBindableUDPConnectionFactory
    {
        IBindableUDPConnection Create();
    }
}
