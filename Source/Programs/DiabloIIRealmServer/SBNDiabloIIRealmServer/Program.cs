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

namespace SBNDiabloIIRealmServer
{
    #region Usings
    using System;
    using Ninject;
    using SharpBattleNet.Runtime;
    using SharpBattleNet.Runtime.Application;
    using SharpBattleNet.DiabloIIRealmServer;
    using SharpBattleNet.Runtime.Networking;
    #endregion

    /// <summary>
    /// Contains the initialization logic for D2RS.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// Called by Windows when the program is started.
        /// </summary>
        /// <param name="args">Parameters passed on the command line.</param>
        /// <returns>
        /// Wheter the program exited successfully or failed. A value of 1
        /// indicates success while a value of 0 indicated failure.
        /// </returns>
        private static int Main(string[] args)
        {
            using(var application = new Application("DiabloIIRealmServer", args))
            {
                application.AddDependencyModule(new DiabloIIRealmServerModule());

                return application.Run();
            }
        }
    }
}

