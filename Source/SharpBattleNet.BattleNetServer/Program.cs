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
    using System.Diagnostics;

    internal static class Program
    {
        private static IKernel _injectionKernel = null;

        private static void PrintHeader(Assembly currentAssembly)
        {
            Console.WriteLine(@"    _  _   ____        _   _   _         _   _      _    ");
            Console.WriteLine(@"  _| || |_| __ )  __ _| |_| |_| | ___   | \ | | ___| |_  ");
            Console.WriteLine(@" |_  .. _ |  _ \ / _` | __| __| |/ _ \  |  \| |/ _ \ __| ");
            Console.WriteLine(@" |_      _| |_) | (_| | |_| |_| |  __/ _| |\  |  __/ |_  ");
            Console.WriteLine(@"   |_||_| |____/ \__,_|\__|\__|_|\___(_)_ | \_|\___|\__| ");

            Console.WriteLine();
            Console.WriteLine("{0} - {1}", currentAssembly.GetAssemblyTitle(), currentAssembly.GetAssemblyFileVersion());
            Console.WriteLine();

            return;
        }

        private static void Start(string[] commandArguments)
        {
            var currentAssembly = Assembly.GetExecutingAssembly();

            Console.Title = String.Format("{ 0} - {1}", currentAssembly.GetAssemblyTitle(), currentAssembly.GetAssemblyFileVersion());
            Console.WindowWidth = 120;
            Console.WindowHeight = 40;

            PrintHeader(currentAssembly);

            _injectionKernel = new StandardKernel(new FrameworkModule("BattleNetServer"), new NetworkModule(), new BattleNetServerModule());

            return;
        }

        private static void Stop()
        {
            _injectionKernel.Dispose();

            return;
        }

        private static void Pause()
        {
            bool enterPressed = false;
            ConsoleKeyInfo lastKey = default(ConsoleKeyInfo);

            Console.WriteLine("Press ENTER to continue...");

            while (false == enterPressed)
            {
                if (true == Console.KeyAvailable)
                {
                    lastKey = Console.ReadKey(true);
                    if (lastKey.Key == ConsoleKey.Enter)
                    {
                        enterPressed = true;
                    }
                }
            }

            return;
        }

        [Conditional("DEBUG")]
        private static void DebugMain(string[] args)
        {
            Start(args);
            Pause();
            Stop();

            return;
        }

        [Conditional("RELEASE")]
        private static void ReleaseMain(string[] args)
        {
            try
            {
                Start(args);
                Pause();
                Stop();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Internal server error:");
                Console.WriteLine(" - {0}", ex.Message);

                if (null != ex.InnerException)
                {
                    Console.WriteLine(" - {0}", ex.InnerException.Message);
                }
            }

            return;
        }

        private static void Main(string[] args)
        {
            DebugMain(args);
            ReleaseMain(args);

            return;
        }
    }
}

