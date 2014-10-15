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

    using Ninject;
    #endregion

    /// <summary>
    /// Used by the application initialization block to scan the command line and apply any changes that were found there.
    /// </summary>
    internal sealed class ApplicationParser : IDisposable
    {
        private readonly IKernel _injectionKernel = null;
        private readonly string[] _arguments = null;

        private bool _disposed = false;

        /// <summary>
        /// Does the actual parsing of the command line.
        /// </summary>
        private void Parse()
        {
            return;
        }

        /// <summary>
        /// Constructs this helper class that scans the command line and apply the options that were found there.
        /// </summary>
        /// <param name="injectionKernel">The Ninject kernel that is used to bind the application.</param>
        /// <param name="arguments">The command line arguments passed by the operating system on the commnad line.</param>
        public ApplicationParser(IKernel injectionKernel, string[] arguments)
        {
            _injectionKernel = injectionKernel;
            _arguments = arguments;

            return;
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
