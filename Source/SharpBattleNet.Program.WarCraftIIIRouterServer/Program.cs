﻿#region Header
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

namespace SharpBattleNet.Program.WarCraftIIIRouterServer
{
    #region Usings
    using System;
    using Ninject;
    using SharpBattleNet.Framework;
    using SharpBattleNet.Framework.Networking;
    using SharpBattleNet.Server.WarCraftIIIRouterServer;
    #endregion

    /// <summary>
    /// The main entry point for the WarCraft III Route Server. Contains the
    /// initialization logic that starts the framework and launches the program
    /// located inside the W3RS library assembly.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// Called by Windows when the application is double clicked inside
        /// explorer or called from the command line. Starts W3RS.
        /// </summary>
        /// <param name="args">Command line arguments</param>
        /// <returns>
        /// A integer value indicating if we exited successfully. A value of
        /// 1 means success while a value of 0 means error.
        /// </returns>
        private static int Main(string[] args)
        {
            FrameworkProgram program = new FrameworkProgram();

            // Configure W3RS via the configuration file and modules.
            program.Configure = kernel =>
                {
                    kernel.Load<WarCraftIIIRouterServerModule>();
                    kernel.Load<NetworkModule>();

                    return "WarCraftIIIRouterServer";
                };

            return program.Run(args);
        }
    }
}
