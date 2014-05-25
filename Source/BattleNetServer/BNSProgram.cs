namespace Reaper.SharpBattleNet.BattleNetServer
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    internal static class BNSProgramReaper
    {
        private static void Setup()
        {
            Console.Title = "SharpBattleNet Server";
            Console.WindowWidth = 120;
            Console.WindowHeight = 40;

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
            Setup();

            Pause();
            return;
        }
    }
}
