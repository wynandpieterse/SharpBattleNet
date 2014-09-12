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

namespace SharpBattleNet.Runtime
{
    #region Usings
    using System;
    using System.Reflection;
    using Ninject;
    using SharpBattleNet.Runtime.Utilities.Extensions;
    using SharpBattleNet.Runtime.Utilities.Debugging;
    #endregion

    /// <summary>
    /// Used by server executables as a common way to enter the system and
    /// start using it.
    /// </summary>
    public sealed class FrameworkProgram
    {
        private IKernel _injectionKernel = null;
        //private IProgram _server = null;

        /// <summary>
        /// Prints the #Battle.Net header to the console.
        /// </summary>
        /// <param name="currentAssembly">The server executable assembly.</param>
        private void PrintHeader(Assembly currentAssembly)
        {
            Guard.AgainstNull(currentAssembly);

            // TODO : Find a way to handle this nicely inside services/daeomons.
            Console.WriteLine(@"    _  _   ____        _   _   _         _   _      _    ");
            Console.WriteLine(@"  _| || |_| __ )  __ _| |_| |_| | ___   | \ | | ___| |_  ");
            Console.WriteLine(@" |_  ..  _|  _ \ / _` | __| __| |/ _ \  |  \| |/ _ \ __| ");
            Console.WriteLine(@" |_      _| |_) | (_| | |_| |_| |  __/_ | |\  |  __/ |_  ");
            Console.WriteLine(@"   |_||_| |____/ \__,_|\__|\__|_|\___(_)_ | \_|\___|\__| ");

            Console.WriteLine();
            Console.WriteLine("{0} - {1}", currentAssembly.GetAssemblyTitle(), currentAssembly.GetAssemblyFileVersion());
            Console.WriteLine();

            return;
        }

        /// <summary>
        /// Starts the program. This creates the IoC container and initializes
        /// it with all the user desired modules. Calls the configure callback
        /// to set up all user required container modules. After this function
        /// the system is in a useable state.
        /// </summary>
        /// <param name="commandArguments">
        /// Parameters passed on the command line.
        /// </param>
        private void Start(string[] commandArguments)
        {
            var currentAssembly = Assembly.GetEntryAssembly();

            Console.Title = string.Format("{0} - {1}", currentAssembly.GetAssemblyTitle(), currentAssembly.GetAssemblyFileVersion());
            Console.WindowWidth = 120;
            Console.WindowHeight = 40;

            PrintHeader(currentAssembly);

            _injectionKernel = new StandardKernel();

            if (null == Configure)
            {
                throw new InvalidProgramException("The configuration property of the program must be set to a valid callback function");
            }
            else
            { 
                string programName = Configure(_injectionKernel);
                if (null == programName)
                {
                    Guard.AgainstNull(programName);
                }
                else
                {
                   // _injectionKernel.Load(new FrameworkModule(programName));
                }
            }

            //_server = _injectionKernel.Get<IProgram>();
            //_server.Start();

            return;
        }

        /// <summary>
        /// Stops the server completely and disposes of the IoC container.
        /// </summary>
        private void Stop()
        {
           // if (null != _server)
            //{
             //   _server.Stop();
           // }

            if (null != _injectionKernel)
            {
                _injectionKernel.Dispose();
            }

            return;
        }

        /// <summary>
        /// Pauses the command line input, waiting for the user to press Enter.
        /// </summary>
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

        /// <summary>
        /// Provides no guards around system calls. Usefull for debug
        /// builds.
        /// </summary>
        /// <param name="args">
        /// Parameters passed on the command line.
        /// </param>
        private void UnguardedRun(string[] args)
        {
            Start(args);
            Pause();
            Stop();

            return;
        }

        /// <summary>
        /// Called by the system when an unhandled exception has occured. Prints
        /// some values to the console to let he user know.
        /// </summary>
        /// <param name="ex">The exception that occured.</param>
        private void UnhandledException(Exception ex)
        {
            // TODO : See if we can maybe integrate with 3rd-party solutions like
            // rocket.io or like that.

            Console.WriteLine();
            Console.WriteLine("INTERNAL SERVER ERROR:");
            Console.WriteLine(" - {0}", ex.Message);

            if (null != ex.InnerException)
            {
                Console.WriteLine(" - {0}", ex.InnerException.Message);
            }

            Console.WriteLine();
            Console.WriteLine(" - Stack Trace");
            Console.Write(ex.StackTrace);

            Console.WriteLine();
            Pause();

            return;
        }

        /// <summary>
        /// Called by the .NET framework when an unhandled exception has occured.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">Contains details about the unhandled exception.</param>
        private void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            UnhandledException((Exception)e.ExceptionObject);

            return;
        }

        /// <summary>
        /// Sets up the system to handle exceptions, then calls the rest of the
        /// initialization pipeline. Usefull in release builds when users dont
        /// have a debugger attached.
        /// </summary>
        /// <param name="args">
        /// Parameters passed on the command line.
        /// </param>
        private void GuardedRun(string[] args)
        {
            try
            {
                AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;

                UnguardedRun(args);
            }
            catch (Exception ex)
            {
                UnhandledException(ex);
            }

            return;
        }

        /// <summary>
        /// User code should set this before calling Run. Called by the
        /// startup code to configure the IoC container with user modules.
        /// </summary>
        public Func<IKernel, string> Configure { get; set; }

        /// <summary>
        /// Starts the system, initializes all IoC modules and call user code.
        /// This function only returns after the server module has exited.
        /// </summary>
        /// <param name="args">
        /// Parameters passed on the command line.
        /// </param>
        /// <returns>
        /// A value indicating if we exited successfully. A valu of 1 indicates
        /// success while a value of 0 indicates failure.
        /// </returns>
        public int Run(string[] args)
        {
            //#if DEBUG
            //UnguardedRun(args);
            //#else
            GuardedRun(args);
            //#endif

            return 0;
        }
    }
}
