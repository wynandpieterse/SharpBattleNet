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
    /// Manages a pool of byte buffer objects. Can acquire new buffer objects
    /// that can help speed up socket and file operations without putting too
    /// much presure on the garbage collector
    /// </summary>
    public interface IBufferPool
    {
        /// <summary>
        /// Gets the name of this buffer pool.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Initializes this buffer pool up to the point of using it.
        /// </summary>
        /// <param name="name">The name of this buffer pool.</param>
        /// <param name="pageSize">
        /// The page size for each allocation from this buffer pool.
        /// </param>
        void Initialize(string name, int pageSize);

        /// <summary>
        /// Requests a new page from the buffer pool that can be used.
        /// </summary>
        /// <returns>
        /// An array segment that contains the byte buffer to be used an the
        /// offset into it.
        /// </returns>
        ArraySegment<byte> Request();

        /// <summary>
        /// Recycles a previously used buffer back into the pool for later
        /// use.
        /// </summary>
        /// <param name="buffer">
        /// The array segment to recycle back into the system.
        /// </param>
        void Recycle(ArraySegment<byte> buffer);
    }
}
