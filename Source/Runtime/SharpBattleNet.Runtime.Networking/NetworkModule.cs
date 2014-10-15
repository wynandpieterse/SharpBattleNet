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

namespace SharpBattleNet.Runtime.Networking
{
    #region Usings
    using System;

    using Ninject;
    using Ninject.Modules;
    using Ninject.Extensions;
    using Ninject.Extensions.Factory;

    using Nini;
    using Nini.Config;

    using SharpBattleNet;
    using SharpBattleNet.Runtime;
    using SharpBattleNet.Runtime.Utilities;
    using SharpBattleNet.Runtime.Utilities.BufferPool;
    using SharpBattleNet.Runtime.Networking;
    using SharpBattleNet.Runtime.Networking.Utilities;
    using SharpBattleNet.Runtime.Networking.Utilities.Collections;
    using SharpBattleNet.Runtime.Networking.Utilities.Collections.Details;
    #endregion

    public sealed class NetworkModule : NinjectModule
    {
        private SocketBufferPool CreateSocketBufferPool()
        {
            var configSource = Kernel.Get<IConfigSource>();
            var configSection = configSource.Configs["Network"];

            int slabSize = configSection.GetInt("BufferSlabSize", 1 * 1024 * 1024);
            if(slabSize < 1024)
            {
                slabSize = 1024;
            }

            int initialSlabs = configSection.GetInt("BufferInitialSlabs", 64);
            if(initialSlabs < 1)
            {
                initialSlabs = 1;
            }

            int subsequentSlabs = configSection.GetInt("BufferSubsequentSlabs", 8);
            if(subsequentSlabs < 1)
            {
                subsequentSlabs = 1;
            }

            return new SocketBufferPool(slabSize, initialSlabs, subsequentSlabs);
        }

        private void BindUtilities()
        {
            Bind<ISocketEventPool>().To<SocketEventPool>().InSingletonScope();
            Bind<ISocketBufferPool>().ToConstant<SocketBufferPool>(CreateSocketBufferPool()).InSingletonScope();

            return;
        }

        public override void Load()
        {
            BindUtilities();

            return;
        }
    }
}

