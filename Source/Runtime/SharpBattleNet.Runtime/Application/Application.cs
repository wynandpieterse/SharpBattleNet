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

namespace SharpBattleNet.Runtime.Application
{
    #region Usings
    using System;
    using System.IO;
    using System.Reflection;
    using System.Collections;
    using System.Collections.Generic;

    using Ninject;
    using Ninject.Modules;

    using SharpBattleNet;
    using SharpBattleNet.Runtime;
    using SharpBattleNet.Runtime.Utilities;
    using SharpBattleNet.Runtime.Utilities.Debugging;
    using SharpBattleNet.Runtime.Utilities.Extensions;
    using SharpBattleNet.Runtime.Application;
    using SharpBattleNet.Runtime.Application.Details;
    #endregion

    /// <summary>
    /// Base application that executables can use to initialize the base layers and call an assembly application to start running after everything is ready for it to use.
    /// This should remain the same for all applications no matter what.
    /// </summary>
    public sealed class Application : IDisposable
    {
        private readonly string _name = "";
        private readonly string[] _arguments = null;

        private bool _disposed = false;
        private List<INinjectModule> _injectionModules = null;
        private string _writeDirectory = null;

        /// <summary>
        /// Initializes a new application.
        /// </summary>
        /// <param name="name">The name of the application. This will be used for stuff like configuration file names and write directory names.</param>
        /// <param name="arguments">Arguments passed on the command line by the operating system.</param>
        public Application(string name, string[] arguments)
        {
            Guard.AgainstEmptyString(name);
            Guard.AgainstNull(arguments);

            _name = name;
            _arguments = arguments;

            _injectionModules = new List<INinjectModule>();

            return;
        }

        /// <summary>
        /// Adds a Ninject module that will have all it's classes registered within Ninject.
        /// </summary>
        /// <param name="module">The module to register.</param>
        public void AddDependencyModule(NinjectModule module)
        {
            if(true == _injectionModules.Contains(module))
            {
                return;
            }

            _injectionModules.Add(module);
            return;
        }

        /// <summary>
        /// Configures the console so that it looks nicer. Increases the size and prints the application name on the title. It also prints out the standard application
        /// header for SharpBattle.net
        /// </summary>
        private void ConfigureConsole()
        {
            var currentAssembly = Assembly.GetEntryAssembly();

            Console.Title = string.Format("{0} - {1}", currentAssembly.GetAssemblyTitle(), currentAssembly.GetAssemblyFileVersion());
            Console.WindowWidth = 120;
            Console.WindowHeight = 40;

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
        /// Configures the write directory for the application. This usually resides within the root ProgramData directory. Every application has write clearance
        /// to that directory so no administration stuff should bother when we write there.
        /// </summary>
        private void ConfigureWriteDirectory()
        {
            string userApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string battleNetDirectory = Path.Combine(userApplicationData, "SharpBattleNet");
            string applicationDirectory = Path.Combine(battleNetDirectory, _name);

            _writeDirectory = applicationDirectory;
            Directory.CreateDirectory(_writeDirectory);

            return;
        }

        /// <summary>
        /// Start the application by creating an injection kernel, doing all the base configuration stuff, and then starting the actual application.
        /// </summary>
        /// <returns>Returns the application exit code back to the operating system.</returns>
        public int UnguardedRun()
        {
            using(var injectionKernel = new StandardKernel())
            {
                ConfigureConsole();
                ConfigureWriteDirectory();

                using(var configuration = new ApplicationConfiguration(injectionKernel, _name, _writeDirectory))
                {
                    using(var logging = new ApplicationLogging(injectionKernel, _name, _writeDirectory))
                    {
                        using (var commandLine = new ApplicationParser(injectionKernel, _arguments))
                        {
                            // Load all application specific modules after the base ones have been created.
                            injectionKernel.Load(_injectionModules);

                            // Start the application
                            using(var application = injectionKernel.Get<IApplicationListener>())
                            {
                                return application.Run();
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Called when an unhandled exception has been caught while running outside of the debugger.
        /// </summary>
        /// <param name="ex">The exception that was caught.</param>
        private void UnhandledException(Exception ex)
        {
            return;
        }

        /// <summary>
        /// Called by the operating system when an unhandled exception has been caught while running outside of the debugger.
        /// </summary>
        /// <param name="sender">The originator of the exception.</param>
        /// <param name="e">The exception that was unhandled.</param>
        private void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            UnhandledException((Exception)e.ExceptionObject);

            return;
        }

        /// <summary>
        /// Run method that guards the inner run loop with a catch block. This is used when the application is runned outside of the debugger to catch exceptions and
        /// print them to the user.
        /// </summary>
        /// <returns>The application return code back to the operating system.</returns>
        private int GuardedRun()
        {
            try
            {
                AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;

                return UnguardedRun();
            }
            catch(Exception ex)
            {
                UnhandledException(ex);
            }

            return -1;
        }

        /// <summary>
        /// Called by the executable owning this applications. Starts the application.
        /// </summary>
        /// <returns>The application exit code.</returns>
        public int Run()
        {
            #if DEBUG
                return UnguardedRun();
            #else
                return GuardedRun();
            #endif
        }

        /// <summary>
        /// Called by the garbage colllector or the application to dispose all managed and unmanaged resources back to the operating system.
        /// </summary>
        /// <param name="disposing">True when the application called the method, false otherwise.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (false == _disposed)
            {
                if (true == disposing)
                {
                    // Dispose managed resources
                }

                // Dispose unmanaged resources
            }

            _disposed = true;

            // Call base dispose

            return;
        }

        /// <summary>
        /// Called when the object is to be disposed, so that all resources can be freed.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);

            return;
        }
    }
}
