namespace SharpBattleNet.Servers.BattleNetServer
{
    using System;
    using System.Reflection;
    using System.Linq;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using SharpBattleNet;
    using SharpBattleNet.Framework;
    using SharpBattleNet.Framework.Extensions;
    using Ninject;
    using SharpBattleNet.Framework.Networking;
    using SharpBattleNet.Server.BattleNetServer;

    internal static class Program
    {
        private static IKernel _injectionKernel = null;

        private static void Start(string[] commandArguments)
        {
            var currentAssembly = Assembly.GetExecutingAssembly();

            Console.Title = String.Format("{0} - {1}", currentAssembly.GetAssemblyTitle(), currentAssembly.GetAssemblyFileVersion());
            Console.WindowWidth = 120;
            Console.WindowHeight = 40;

            _injectionKernel = new StandardKernel(new FrameworkModule("BattleNetServer"), new NetworkModule(), new BattleNetServerModule());

            return;
        }

        private static void Stop()
        {
            Runner.Stop();

            return;
        }

        private static void Pause()
        {
            bool enterPressed = false;
            ConsoleKeyInfo lastKey = default(ConsoleKeyInfo);

            Console.WriteLine("Press ENTER to continue...");

            while(false == enterPressed)
            {
                if(true == Console.KeyAvailable)
                {
                    lastKey = Console.ReadKey(true);
                    if(lastKey.Key == ConsoleKey.Enter)
                    {
                        enterPressed = true;
                    }
                }
            }

            return;
        }

        private static void Main(string[] args)
        {
            Start(args);
            Pause();
            Stop();
            return;
        }
    }
}

