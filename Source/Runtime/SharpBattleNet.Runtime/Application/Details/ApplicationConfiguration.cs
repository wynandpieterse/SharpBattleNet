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

namespace SharpBattleNet.Runtime.Application.Details
{
    #region Usings
    using System;
    using System.IO;

    using Ninject;

    using Nini;
    using Nini.Config;
    #endregion

    internal sealed class ApplicationConfiguration : IDisposable
    {
        private readonly IKernel _injectionKernel = null;
        private readonly string _applicationName = "";
        private readonly string _writeDirectory = "";

        private bool _disposed = false;

        private void Configure()
        {
            string configurationFile = _applicationName + ".ini";
            string configurationBasePath = "../Configuration/" + configurationFile;
            string configurationPath = Path.Combine(_writeDirectory, configurationFile);
            DateTime configurationBaseTime = default(DateTime);
            DateTime configurationTime = default(DateTime);

            try
            {
                if (false == File.Exists(configurationPath))
                {
                    File.Copy(configurationBasePath, configurationPath);
                }
                else
                {
                    configurationBaseTime = File.GetLastWriteTimeUtc(configurationBasePath);
                    configurationTime = File.GetLastWriteTimeUtc(configurationPath);

                    if (configurationBaseTime > configurationTime)
                    {
                        File.Delete(configurationPath);
                        File.Copy(configurationBasePath, configurationPath);
                    }
                }

                _injectionKernel.Bind<IConfigSource>().ToConstant(new IniConfigSource(configurationPath)).InSingletonScope();
            }
            catch(Exception)
            {
                Console.WriteLine("Failed to initialize user-specific configuration. Using base configuration as default.");
                _injectionKernel.Bind<IConfigSource>().ToConstant(new IniConfigSource(configurationBasePath)).InSingletonScope();
            }

            return;
        }

        public ApplicationConfiguration(IKernel injectionKernel, string applicationName, string writeDirectory)
        {
            _injectionKernel = injectionKernel;
            _applicationName = applicationName;
            _writeDirectory = writeDirectory;

            Configure();

            return;
        }

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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);

            return;
        }
    }
}
