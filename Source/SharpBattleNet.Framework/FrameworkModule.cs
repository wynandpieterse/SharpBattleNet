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
    using System.IO;
    using Ninject;
    using Ninject.Modules;
    using SharpBattleNet.Framework.Utilities.Debugging;
    using Ninject.Extensions.Factory;
    using System.Collections.Generic;
    #endregion

    /// <summary>
    /// Provides common IoC modules that is used by most of the system. Provides
    /// configuration and logging modules built into one.
    /// </summary>
    public sealed class FrameworkModule : NinjectModule
    {
        private readonly string _applicationName = "";

        private string _writeDirectory = "";
        private bool _writeDirectorySuccessfull = false;

        /// <summary>
        /// Constructs an empty <see cref="FrameworkModule"/>.
        /// </summary>
        /// <param name="applicationName">
        /// The application name that is calling us.
        /// </param>
        public FrameworkModule(string applicationName)
        {
            Guard.AgainstNull(applicationName);
            Guard.AgainstEmptyString(applicationName);

            _applicationName = applicationName;

            return;
        }

        /// <summary>
        /// Creates the application write directory if it does not exists.
        /// </summary>
        private void ConfigureWriteDirectory()
        {
            try
            {
                _writeDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "SharpBattleNet");
                _writeDirectory = Path.Combine(_writeDirectory, _applicationName);

                Directory.CreateDirectory(_writeDirectory);

                _writeDirectorySuccessfull = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to create write directory for {0}", _applicationName);
                Console.WriteLine(ex.Message);

                _writeDirectorySuccessfull = false;
            }

            return;
        }

        /// <summary>
        /// Called by Ninject to configure this module.
        /// </summary>
        public override void Load()
        {
            ConfigureWriteDirectory();
            return;
        }
    }
}

