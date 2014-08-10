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

namespace SharpBattleNet.Framework
{
    #region Usings
    using System;
    using System.Reflection;

    using Ninject;

    using SharpBattleNet.Framework;
    using SharpBattleNet.Framework.Utilities.Extensions;
    using SharpBattleNet.Framework.Utilities.Debugging;
    #endregion

    public sealed class FrameworkProgram
    {
        private string _programName = "";
        private IKernel _injectionKernel = null;

        private void PrintHeader(Assembly currentAssembly)
        {
            Guard.AgainstNull(currentAssembly);

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

        private void Start(string[] commandArguments)
        {
            var currentAssembly = Assembly.GetEntryAssembly();

            Console.Title = string.Format("{0} - {1}", currentAssembly.GetAssemblyTitle(), currentAssembly.GetAssemblyFileVersion());
            Console.WindowWidth = 120;
            Console.WindowHeight = 40;

            PrintHeader(currentAssembly);

            _injectionKernel = new StandardKernel(new FrameworkModule(_programName));

            return;
        }

        private void Stop()
        {
            _injectionKernel.Dispose();

            return;
        }

        private void Pause()
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

        private void UnguardedRun(string[] args)
        {
            Start(args);
            Pause();
            Stop();

            return;
        }

        private void GuardedRun(string[] args)
        {
            try
            {
                UnguardedRun(args);
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

        public int Run(string programName, string[] args)
        {
            Guard.AgainstNull(programName);

            _programName = programName;

            #if DEBUG
            UnguardedRun(args);
            #else
            GuardedRun(args);
            #endif

            return 0;
        }
    }
}
