﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpBattleNet.Runtime.Application.Details.Service
{
    internal sealed class ServiceApplicationHandler : BaseApplicationHandler
    {
        private bool _disposed = false;

        public ServiceApplicationHandler(ICommandLineParser commandLineParser, IApplicationListener applicationListener)
            : base(commandLineParser, applicationListener)
        {
            return;
        }

        public override int Run(string[] arguments)
        {
            Start();
            Stop();
            return 0;
        }

        protected override void Dispose(bool disposing)
        {
            if(false == _disposed)
            {
                _disposed = true;
            }

            base.Dispose(disposing);
            return;
        }
    }
}