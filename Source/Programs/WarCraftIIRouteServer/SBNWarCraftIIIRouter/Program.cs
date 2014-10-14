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

namespace SBNWarCraftIIIRouteServer
{
    #region Usings
    using System;
    using Ninject;
    using SharpBattleNet.Runtime;
    using SharpBattleNet.Runtime.Networking;
    using SharpBattleNet.WarCraftIIIRouterServer;
    using SharpBattleNet.Runtime.Application;
    #endregion

    /// <summary>
    /// Main entry point for WarCraft III router servers. Contains the initialization logic that calls out to the framework
    /// binary and initializes all the stuff that the route server requires.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// Called by Windows when the program is started. This is the main entry point for WarCraft III route server.
        /// </summary>
        /// <param name="args">
        /// Command line parameters passed in from windows. These can be used to configure the program from the command
        /// line if it is required.
        /// </param>
        /// <returns>
        /// An integer value that states wheter the program exited successfully or not. A value of 1 indicates success
        /// while a value of 0 indicates failure.
        /// </returns>
        private static int Main(string[] args)
        {
            using(var application = new Application("WarCraftIIIRouterServer", args))
            {
                application.AddDependencyModule(new WarCraftIIIRouterServerModule());

                return application.Run();
            }
        }
    }
}

