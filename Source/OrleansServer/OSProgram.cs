namespace Reaper.SharpBattleNet.BattleNetServer
{
    using System;
    using Orleans;

    internal static class OSProgram
    {
        private static OrleansHostWrapper _hostWrapper = null;
        private static AppDomain _hostDomain = null;

        private static void Startup(string[] commandArguments)
        {
            Console.Title = "SharpBattleNet - Orleans Server";
            Console.WindowWidth = 120;
            Console.WindowHeight = 40;

            InitializeOrleansHost(commandArguments);

            return;
        }

        private static void InitializeOrleansHost(string[] commandArguments)
        {
            AppDomainSetup appSetup = new AppDomainSetup()
            {
                AppDomainInitializer = StartupSilo,
                AppDomainInitializerArguments = commandArguments,
            };

            _hostDomain = AppDomain.CreateDomain("OrleansHost", null, appSetup);

            OrleansClient.Initialize("../Configuration/OrleansServer/OrleansConfigurationClient.xml");

            return;
        }

        private static void StartupSilo(string[] args)
        {
            _hostWrapper = new OrleansHostWrapper(args);

            if(false == _hostWrapper.Run())
            {
                Console.WriteLine("Failed to initialize Orleans host.");
            }

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

        private static void Shutdown()
        {
            _hostDomain.DoCallBack(ShutdownSilo);
            return;
        }

        private static void ShutdownSilo()
        {
            if(null != _hostWrapper)
            {
                _hostWrapper.Dispose();
                GC.SuppressFinalize(_hostWrapper);
            }

            return;
        }

        private static void Main(string[] args)
        {
            Startup(args);
            Pause();
            Shutdown();
            return;
        }
    }
}