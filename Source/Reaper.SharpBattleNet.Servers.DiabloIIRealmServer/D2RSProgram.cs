namespace Reaper.SharpBattleNet.Servers.DiabloIIRealmServer
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    internal static class D2RSProgram
    {
        private static void Start(string[] commandArguments)
        {
            Console.Title = "SharpBattleNet - Diablo II Realm Server";
            Console.WindowWidth = 120;
            Console.WindowHeight = 40;

            Runner.Start(commandArguments);

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
