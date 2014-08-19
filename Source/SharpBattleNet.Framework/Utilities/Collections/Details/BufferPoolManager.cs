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
    using System.Collections.Concurrent;
    using NLog;
    using SharpBattleNet.Framework.Utilities.Debugging;
    #endregion

    /// <summary>
    /// Implements the interface of <see cref="IBufferPoolManager"/>.
    /// </summary>
    internal class BufferPoolManager : IBufferPoolManager
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly ConcurrentDictionary<string, IBufferPool> _pools = null;
        private readonly IBufferPoolFactory _bufferFactory = null;

        /// <summary>
        /// Constructs an empty <see cref="BufferPoolManager"/>.
        /// </summary>
        /// <param name="bufferFactory">
        /// A factory interface that can create <see cref="IBufferPool"/> objects.
        /// </param>
        public BufferPoolManager(IBufferPoolFactory bufferFactory)
        {
            Guard.AgainstNull(bufferFactory);

            _pools = new ConcurrentDictionary<string, IBufferPool>();
            _bufferFactory = bufferFactory;

            return;
        }

        #region IBufferPoolManager Members

        /// <summary>
        /// Creates a new buffer pool with the specified name and the allocation
        /// size of each page.
        /// </summary>
        /// <param name="name">The name of the buffer pool.</param>
        /// <param name="pageSize">
        /// The page size for each allocation from the buffer pool.
        /// </param>
        /// <returns>A reference to the buffer pool that was created.</returns>
        public IBufferPool Create(string name, int pageSize)
        {
            IBufferPool poolToCreate = null;

            Guard.AgainstEmptyString(name);

            if (0 == pageSize)
            {
                throw new ArgumentException("Page size cannot be zero", "pageSize");
            }

            if((8 * 1024) < pageSize)
            {
                throw new ArgumentException("Page size cannot be more than 8KB in size", "pageSize");
            }

            if (true == _pools.ContainsKey(name))
            {
                throw new InvalidOperationException(string.Format("The buffer pool dictionary already contains a buffer pool with the name {0}", name));
            }

            poolToCreate = _bufferFactory.Create();

            poolToCreate.Initialize(name, pageSize);

            if (false == _pools.TryAdd(name, poolToCreate))
            {
                throw new InvalidProgramException(string.Format("Failed to add the buffer pool \'{0}\' to the pool dictionary", name));
            }
            else
            {
                _logger.Trace("Successfully created buffer pool : {0}", name);
            }

            return null;
        }

        /// <summary>
        /// Retrieves a previously created buffer pool. Will fail if the buffer
        /// pool does not exist.
        /// </summary>
        /// <param name="name">
        /// The name of the <see cref="IBufferPool"/> to get.
        /// </param>
        /// <returns>
        /// A reference to the requested <see cref="IBufferPool"/>.
        /// </returns>
        public IBufferPool Get(string name)
        {
            IBufferPool pool = null;

            Guard.AgainstEmptyString(name);

            if (false == _pools.TryGetValue(name, out pool))
            {
                throw new InvalidOperationException(string.Format("The pool {0} does not exist in the buffer pool manager", name));
            }

            return pool;
        }

        #endregion
    }
}
