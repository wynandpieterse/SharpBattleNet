using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SharpBattleNet.Runtime.Utilities.Debugging;
using SharpBattleNet.Runtime.Utilities.Extensions;

namespace SharpBattleNet.Runtime.Application.Details.Console
{
    internal sealed class ConsoleApplicationHandler : ApplicationHandler
    {
        private bool _disposed = false;

        public ConsoleApplicationHandler(ICommandLineParser commandLineParser, IApplicationListener applicationListener)
            : base(commandLineParser, applicationListener)
        {
            return;
        }

        private void SetupConsole()
        {
            var currentAssembly = Assembly.GetEntryAssembly();

            System.Console.Title = string.Format("{0} - {1}", currentAssembly.GetAssemblyTitle(), currentAssembly.GetAssemblyFileVersion());
            System.Console.WindowWidth = 120;
            System.Console.WindowHeight = 40;

            return;
        }

        protected override void Start()
        {
            SetupConsole();

            base.Start();
            return;
        }

        private void RunCommandLoop()
        {
            System.Console.ReadLine();
            return;
        }

        protected override void Stop()
        {
            base.Stop();
            return;
        }

        public override int Run(string[] arguments)
        {
            Start();
            RunCommandLoop();
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
