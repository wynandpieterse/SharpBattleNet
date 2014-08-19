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

namespace SharpBattleNet.Framework.Utilities.Collections
{
    #region Usings
    using System;
    #endregion

    /// <summary>
    /// Manages buffer pools. Request a buffer pool, then use the methods inside
    /// <see cref="IBufferPool"/> to manage the buffers inside it.
    /// </summary>
    public interface IBufferPoolManager
    {
        /// <summary>
        /// Creates a brand new buffer pool with allocations handeling from the
        /// page size.
        /// </summary>
        /// <param name="name">The globally unique buffer pool name.</param>
        /// <param name="pageSize">
        /// The size of each allocation from the pool.
        /// </param>
        /// <returns>
        /// A refernce to the <see cref="IBufferPool"/> that was created.
        /// </returns>
        IBufferPool Create(string name, int pageSize);

        /// <summary>
        /// Requests a previously created buffer pool. Will fail if the pool does
        /// not exist.
        /// </summary>
        /// <param name="name">The name of the pool to acquire.</param>
        /// <returns>
        /// A reference to the <see cref="IBufferPool"/> with the passed in name.
        /// </returns>
        IBufferPool Get(string name);
    }
}
