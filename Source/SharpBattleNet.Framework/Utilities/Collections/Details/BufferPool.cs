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

namespace SharpBattleNet.Framework.Utilities.Collections.Details
{
    #region Usings
    using System;
    using NLog;
    using SharpBattleNet.Framework.Utilities.Debugging;
    using System.Collections.Concurrent;
    #endregion

    /// <summary>
    /// Implements the interface of <see cref="IBufferPool"/>.
    /// </summary>
    internal class BufferPool : IBufferPool
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private string _name = "";
        private int _pageSize = 0;
        private byte[] _buffer = null;
        private ConcurrentStack<int> _freeIndices = null;

        /// <summary>
        /// Creates an empty <see cref="BufferPool"/> object.
        /// </summary>
        public BufferPool()
        {
            _freeIndices = new ConcurrentStack<int>();

            return;
        }

        #region IBufferPool Members

        /// <summary>
        /// Request the name of this buffer pool.
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
        }

        /// <summary>
        /// Initializes the buffer pool with the desired name and requested
        /// page allocation size.
        /// </summary>
        /// <param name="name">The name of this pool.</param>
        /// <param name="pageSize">
        /// The size of each allocation from this buffer pool.
        /// </param>
        public void Initialize(string name, int pageSize)
        {
            Guard.AgainstEmptyString(name);

            if (0 == pageSize)
            {
                throw new ArgumentException("Page size cannot be zero", "pageSize");
            }

            if ((8 * 1024) < pageSize)
            {
                throw new ArgumentException("Page size cannot be more than 8KB in size", "pageSize");
            }

            _name = name;
            _pageSize = pageSize;

            // TODO : Make a pool that can grow and shrink as needed, then this
            // can be done away with, and we have a more scalable solution. This
            // implementation can currently handle only a 1024 allocations from
            // it. So if more clients, eg. connects to the server, the receive
            // pool will be exhausted very quickly.
            _buffer = new byte[pageSize * 1024];

            for (int index = 0; index < 1024; index++)
            {
                _freeIndices.Push(index);
            }

            return;
        }

        /// <summary>
        /// Recycles a previous allocated buffer back into this buffer pool
        /// for later use.
        /// </summary>
        /// <param name="buffer">
        /// The buffer to recycle back into this pool.
        /// </param>
        public void Recycle(ArraySegment<byte> buffer)
        {
            int index = 0;

            if (null == buffer.Array)
            {
                throw new InvalidOperationException("The passed in buffer array cannot be null");
            }

            if (_buffer != buffer.Array)
            {
                throw new InvalidOperationException("The passed in buffer did not originate from this pool");
            }

            index = buffer.Offset / 1024;
            _freeIndices.Push(index);

            return;
        }

        /// <summary>
        /// Request a new buffer from this pool that is exactly page size big.
        /// </summary>
        /// <returns>The requested buffer, ready to be used.</returns>
        public ArraySegment<byte> Request()
        {
            int index = 0;

            if (false == _freeIndices.TryPop(out index))
            {
                throw new InvalidOperationException("The buffer pool has exhausted its resources");
            }

            Array.Clear(_buffer, index * 1024, _pageSize);
            return new ArraySegment<byte>(_buffer, index * 1024, _pageSize);
        }

        #endregion
    }
}
