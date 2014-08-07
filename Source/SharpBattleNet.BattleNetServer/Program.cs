namespace SharpBattleNet.Servers.BattleNetServer
{
    using System;
    using System.Reflection;
    using Framework;
    using Framework.Extensions;
    using Ninject;
    using Framework.Networking;
    using Server.BattleNetServer;

    internal static class Program
    {
        private static IKernel _injectionKernel = null;

        private static void PrintHeader(Assembly currentAssembly)
        {
            Console.WriteLine(@"    _  _   ____        _   _   _         _   _      _    ");
            Console.WriteLine(@"  _| || |_| __ )  __ _| |_| |_| | ___   | \ | | ___| |_  ");
            Console.WriteLine(@" |_  .. _ |  _ \ / _` | __| __| |/ _ \  |  \| |/ _ \ __| ");
            Console.WriteLine(@" |_      _| |_) | (_| | |_| |_| |  __/_ | |\  |  __/ |_  ");
            Console.WriteLine(@"   |_||_| |____/ \__,_|\__|\__|_|\___(_)_ | \_|\___|\__| ");

            Console.WriteLine();
            Console.WriteLine("{0} - {1}", currentAssembly.GetAssemblyTitle(), currentAssembly.GetAssemblyFileVersion());
            Console.WriteLine();

            return;
        }

        private static void Start(string[] commandArguments)
        {
            var currentAssembly = Assembly.GetExecutingAssembly();

            Console.Title = string.Format("{0} - {1}", currentAssembly.GetAssemblyTitle(), currentAssembly.GetAssemblyFileVersion());
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

        private static void Run(string[] args)
        {
            Start(args);
            Pause();
            Stop();

            return;
        }

        private static void GuardedRun(string[] args)
        {
            try
            {
                Run(args);
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
            #if DEBUG == true
            Run(args);
            #else
            GuardedRun(args);
            #endif

            return;
        }
    }
}

