﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpBattleNet.Runtime.Application.Details
{
    internal abstract class BaseApplicationHandler : IApplicationHandler
    {
        public int Run(string[] arguments)
        {
            return 0;
        }
    }
}
