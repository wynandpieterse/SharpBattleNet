#region Header
//
//    _  _   ____        _   _   _         _   _      _   
//  _| || |_| __ )  __ _| |_| |_| | ___   | \ | | ___| |_ 
// |_  .. _ |  _ \ / _` | __| __| |/ _ \  |  \| |/ _ \ __|
// |_      _| |_) | (_| | |_| |_| |  __/_ | |\  |  __/ |_ 
//   |_||_| |____/ \__,_|\__|\__|_|\___(_)_ | \_|\___|\__|
//
// The MIT License
// 
// Copyright(c) 2014 Wynand Pieters. https://github.com/wpieterse/SharpBattleNet

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
#endregion

namespace SharpBattleNet.Servers.DiabloIIRealmServer
{
    #region Usings
    using System;
    using System.Reflection;

    using Ninject;

    using SharpBattleNet.Framework;
    using SharpBattleNet.Framework.Utilities.Extensions;
    using SharpBattleNet.Server.DiabloIIRealmServer;
    using SharpBattleNet.Server.DiabloIIRealmServer.Server;
    #endregion

    internal static class Program
    {
        private static IKernel _injectionKernel = null;
        private static IDiabloIIRealmServerProgram _server = null;

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

            Console.Title = String.Format("{0} - {1}", currentAssembly.GetAssemblyTitle(), currentAssembly.GetAssemblyFileVersion());
            Console.WindowWidth = 120;
            Console.WindowHeight = 40;

            PrintHeader(currentAssembly);

            _injectionKernel = new StandardKernel(new FrameworkModule("DiabloIIRealmServer"), new DiabloIIRealmServerModule());
            _server = _injectionKernel.Get<IDiabloIIRealmServerProgram>();

            _server.Start();

            return;
        }

        private static void Stop()
        {
            _server.Stop();

            _injectionKernel.Dispose();

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
                // Can't really use NLogger here because I dont know if it was configured correctly
                // by this time
                Console.WriteLine();
                Console.WriteLine("INTERNAL SERVER ERROR:");
                Console.WriteLine(" - {0}", ex.Message);

                if (null != ex.InnerException)
                {
                    Console.WriteLine(" - {0}", ex.InnerException.Message);
                }

                Console.WriteLine();
                Pause();
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

