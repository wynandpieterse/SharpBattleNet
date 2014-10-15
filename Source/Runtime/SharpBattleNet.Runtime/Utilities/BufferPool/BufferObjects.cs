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

namespace SharpBattleNet.Runtime.Utilities.BufferPool
{
    #region Usings
    using System;
    using System.Threading;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime;
    using System.Runtime.CompilerServices;
    #endregion

    public sealed class ManagedBuffer : IBuffer
    {
        private bool disposed = false;

        IList<IMemoryBlock> memoryBlocks;
        byte[] slabArray;
        readonly long size;

        internal ManagedBuffer(IList<IMemoryBlock> allocatedMemoryBlocks)
        {
            if (allocatedMemoryBlocks == null) throw new ArgumentNullException("allocatedMemoryBlocks");
            if (allocatedMemoryBlocks.Count == 0) throw new ArgumentException("allocatedMemoryBlocks cannot be empty"); 
            memoryBlocks = allocatedMemoryBlocks;
            size = 0;
            for (int i = 0; i < allocatedMemoryBlocks.Count; i++)
            {
                size += allocatedMemoryBlocks[i].Length;
            }
            slabArray = null;
        }


        internal ManagedBuffer(IMemorySlab slab)
        {
            if (slab == null) throw new ArgumentNullException("slab");
            memoryBlocks = null;
            this.slabArray = slab.Array;
            size = 0;
        }


        public bool IsDisposed
        {
            get { return disposed; }
        }

        public long Size
        {
            get { return size; }
        }

        public int SegmentCount
        {
            get { return memoryBlocks == null ? 1 : memoryBlocks.Count; }
        }

        internal IList<IMemoryBlock> MemoryBlocks
        {
            get { return memoryBlocks ?? new List<IMemoryBlock>(); }
        } 

        public IList<ArraySegment<byte>> GetSegments()
        {
            if (disposed) throw new ObjectDisposedException(this.ToString());
            return GetSegments(0, this.Size);            
        }

        public IList<ArraySegment<byte>> GetSegments(long length)
        {
            if (disposed) throw new ObjectDisposedException(this.ToString());
            return GetSegments(0, length);
        }

        public IList<ArraySegment<byte>> GetSegments(long offset, long length)
        {
            if (disposed) throw new ObjectDisposedException(this.ToString());
            if (length > this.Size || length < 0)
            {
                throw new ArgumentOutOfRangeException("length");
            }

            if ((offset >= this.Size && this.Size != 0) || offset < 0)
            {
                throw new ArgumentOutOfRangeException("offset");
            }

            //TODO: Check if offset + length > this.Size

            IList<ArraySegment<byte>> result = new List<ArraySegment<byte>>();
            if (this.Size == 0)
            {
                result.Add(new ArraySegment<byte>(slabArray, 0, 0));
                return result;
            }
            else
            {
                //Identify which memory block contains the index to offset and the block's inner offset to sought offset                
                int startBlockIndex;
                long startBlockOffSet;
                FindBlockWithOffset(offset, out startBlockIndex, out startBlockOffSet);

                //Get first segment
                long totalLength = 0;
                {
                    IMemoryBlock startBlock = memoryBlocks[startBlockIndex];
                    if (startBlock.Length >= (startBlockOffSet + length))
                    {
                        //Block can hold entire desired length
                        result.Add(new ArraySegment<byte>(startBlock.Slab.Array, (int)(startBlockOffSet + startBlock.StartLocation), (int)length));
                        return result;
                    }
                    else
                    {
                        //Block can only hold part of desired length
                        result.Add(new ArraySegment<byte>(startBlock.Slab.Array, (int)(startBlockOffSet + startBlock.StartLocation), (int)(startBlock.Length - startBlockOffSet)));
                        totalLength += (startBlock.Length - startBlockOffSet);
                    }
                }

                //Get next set of segments
                IMemoryBlock block;
                for (int i = startBlockIndex + 1; i < memoryBlocks.Count; i++)
                {
                    block = memoryBlocks[i];
                    if (block.Length >= (length - totalLength))
                    {
                        //Block can hold the remainder of desired length
                        result.Add(new ArraySegment<byte>(block.Slab.Array, (int)(block.StartLocation), (int)(length - totalLength)));
                        return result;
                    }
                    else
                    {
                        //Block can only hold only part of the remainder of desired length
                        result.Add(new ArraySegment<byte>(block.Slab.Array, (int)(block.StartLocation), (int)block.Length));
                        totalLength += block.Length;
                    }

                }

                System.Diagnostics.Debug.Assert(true, "Execution should never reach this point, the returns above should be responsible for returning result");
                return result;
            }
        }

        public void CopyTo(byte[] destinationArray)
        {
            if (disposed) throw new ObjectDisposedException(this.ToString());

            CopyTo(destinationArray, 0, this.Size);
        }

        public void CopyTo(byte[] destinationArray, long destinationIndex, long length)
        {
            if (disposed) throw new ObjectDisposedException(this.ToString());
            if (destinationArray == null) throw new ArgumentNullException("destinationArray");
            if (length > this.Size) throw new ArgumentException("length is larger than buffer size");
            if (destinationIndex + length > destinationArray.Length) throw new ArgumentException("destinationIndex + length is greater than length of destinationArray");
            if (this.Size == 0) return;

            long bytesCopied = 0;
            IMemoryBlock block;
            for (int i = 0; i < memoryBlocks.Count; i++)
            {
                block = memoryBlocks[i];
                if (block.Length >= (length - bytesCopied))
                {
                    //This block can copy out the remainder of desired length
                    Array.Copy(block.Slab.Array, block.StartLocation, destinationArray, destinationIndex + bytesCopied, length - bytesCopied);
                    return;
                }
                else
                {
                    //This block can only copy out part of desired length
                    Array.Copy(block.Slab.Array, block.StartLocation, destinationArray, destinationIndex + bytesCopied, block.Length);
                    bytesCopied += block.Length;
                }
            }

            System.Diagnostics.Debug.Assert(true, "Execution should never reach this point, the returns above should be responsible for exiting the method");
        }

        [Obsolete("Use the FillWith method instead -- this method will be removed in a later version", true)]
        public void CopyFrom(byte[] sourceArray)
        {
            FillWith(sourceArray);
        }

        [Obsolete("Use the FillWith method instead -- this method will be removed in a later version", true)]
        public void CopyFrom(byte[] sourceArray, long sourceIndex, long length)
        {
            FillWith(sourceArray, sourceIndex, length);
        }

        public void FillWith(byte[] sourceArray)
        {
            if (disposed) throw new ObjectDisposedException(this.ToString());

            FillWith(sourceArray, 0, sourceArray.Length);
        }

        public void FillWith(byte[] sourceArray, long sourceIndex, long length)
        {
            if (disposed) throw new ObjectDisposedException(this.ToString());
            if (sourceArray == null) throw new ArgumentNullException("sourceArray");
            if (length > (this.Size - sourceIndex)) throw new ArgumentException("length will not fit in the buffer");
            if (this.Size == 0) return;

            //NOTE: try not to keep this method as simple as possible, it's can be called from IBuffer.GetBuffer
            //and we do not want new unexpected exceptions been thrown.

            long bytesCopied = 0;
            IMemoryBlock block;
            for (int i = 0; i < memoryBlocks.Count; i++)
            {
                block = memoryBlocks[i];
                if (block.Length >= (length - bytesCopied))
                {
                    //This block can be filled with the remainder of desired length
                    Array.Copy(sourceArray, sourceIndex + bytesCopied, block.Slab.Array, block.StartLocation, length - bytesCopied);
                    return;
                }
                else
                {
                    //This block can be filled with only part of desired length
                    Array.Copy(sourceArray, sourceIndex + bytesCopied, block.Slab.Array, block.StartLocation, block.Length);
                    bytesCopied += block.Length;
                }
            }

            System.Diagnostics.Debug.Assert(true, "Execution should never reach this point, the returns above should be responsible for exiting the method");

        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!disposed)
                {
                    disposed = true;

                    if (memoryBlocks != null)
                    {
                        for (int i = 0; i < memoryBlocks.Count; i++)
                        {
                            try
                            {
                                memoryBlocks[i].Slab.Free(memoryBlocks[i]);
                            }
                            catch
                            {
                                //Suppress exception in release mode
                                #if DEBUG
                                    throw;
                                #endif
                            }
                        }

                        //Remove all references to memory blocks and their underlying slabs/arrays
                        memoryBlocks = null;
                    }

                    if (slabArray != null) slabArray = null;
                }
            }
        }

        //This helper method identifies the block that holds an 'outer' offset and the inner offset within that block
        private void FindBlockWithOffset(long offset, out int blockIndex, out long blockOffSet)
        {
            long totalScannedLength = 0;
            blockIndex = 0;
            blockOffSet = 0;
            for (int i = 0; i < memoryBlocks.Count; i++)
            {
                if (offset < totalScannedLength + memoryBlocks[i].Length)
                {
                    //Found block;
                    blockIndex = i;
                    //Calculate start offset within block
                    blockOffSet = offset - totalScannedLength;
                    break;
                }
                totalScannedLength += memoryBlocks[i].Length;
            }
        }

    }

    public class BufferPool : IBufferPool
    {
        public const int MinimumSlabSize = 92160; //90 KB to force slab into LOH
        public const long MaximumSlabSize = ((long)int.MaxValue) + 1; 
        private readonly IMemorySlab firstSlab;
        private readonly long slabSize;
        private readonly int initialSlabs, subsequentSlabs;
        private readonly object syncSlabList = new object(); //synchronizes access to the array of slabs
        private readonly object syncNewSlab = new object(); //synchronizes access to new slab creation
        private readonly List<IMemorySlab> slabs = new List<IMemorySlab>();
        private int singleSlabPool; //-1 or 0, used for faster access if only one slab is available

        private const int MAX_SEGMENTS_PER_BUFFER = 16; //Maximum number of segments in a buffer.

        public BufferPool(long slabSize, int initialSlabs, int subsequentSlabs)
        {

            if (slabSize < 1) throw new ArgumentException("slabSize must be equal to or greater than 1");
            if (initialSlabs < 1) throw new ArgumentException("initialSlabs must be equal to or greater than 1");
            if (subsequentSlabs < 1) throw new ArgumentException("subsequentSlabs must be equal to or greater than 1");
            if (slabSize > MaximumSlabSize) throw new ArgumentException("slabSize cannot be larger BufferPool.MaximumSlabSize");

            this.slabSize = slabSize > MinimumSlabSize ? slabSize : MinimumSlabSize;
            this.initialSlabs = initialSlabs;
            this.subsequentSlabs = subsequentSlabs;

            // lock is unnecessary in this instance constructor
            //lock (syncSlabList)
            //{
                if (slabs.Count == 0)
                {
                    SetSingleSlabPool(initialSlabs == 1); //Assume for optimization reasons that it's a single slab pool if the number of initial slabs is 1

                    for (int i = 0; i < initialSlabs; i++)
                    {
                        slabs.Add(new MemorySlab(slabSize, this));
                    }

                    firstSlab = slabs[0];
                }
            //}
        }

        public int InitialSlabs
        {
            get { return initialSlabs; }
        }

        public int SubsequentSlabs
        {
            get { return subsequentSlabs; }
        }

        public long SlabSize
        {
            get { return slabSize; }
        }

        internal long SlabCount
        {
            //NOTE: Some synchronization might be necessary if this method becomes public
            get { return slabs.Count; }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private bool GetSingleSlabPool()
        {
            return singleSlabPool == -1 ? true : false;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void SetSingleSlabPool(bool value)
        {
            //TODO: Is this Interlocked.Exchange here necessary?, singleSlabPool is a 32-bit value:
            Interlocked.Exchange(ref singleSlabPool, value == true ? -1 : 0);
        }

        public IBuffer GetBuffer(long size)
        {
            return GetBuffer(size, null);
        }

        public IBuffer GetBuffer(long size, byte[] filledWith)
        {
            if (size < 0) throw new ArgumentException("size must be greater than 0");

            //TODO: If size is larger than 16 * SlabSize (or MaxNumberOfSegments * SlabSize) then throw exception saying you can't have a buffer greater than 16 times Slab size
            //Write test for this

            //Make sure filledWith can fit into the requested buffer, so that we do not allocate a buffer and then
            //an exception is thrown (when IBuffer.FillWith() is called) before the buffer is returned.
            if (filledWith != null)
            {
                if (filledWith.LongLength > size) throw new ArgumentException("Length of filledWith array cannot be larger than desired buffer size");
                if (filledWith.LongLength == 0) filledWith = null;

                //TODO: Write test that will test that IBuffer.FillWith() doesn't throw an exception (and that buffers aren't allocated) in this method
            }

            if (size == 0)
            {
                //Return an empty buffer
                return new ManagedBuffer(firstSlab);
            }

            List<IMemoryBlock> allocatedBlocks = new List<IMemoryBlock>();

            //TODO: Consider the performance penalty involved in making the try-catch below a constrained region
            //RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                IMemorySlab[] slabArr;
                long allocdLengthTally = 0;

                if (GetSingleSlabPool())
                {
                    //Optimization: Chances are that there'll be just one slab in a pool, so access it directly 
                    //and avoid the lock statement involved while creating an array of slabs.

                    //Note that even if singleSlabPool is inaccurate, this method will still work properly.
                    //The optimization is effective because singleSlabPool will be accurate majority of the time.

                    slabArr = new IMemorySlab[] { firstSlab };
                    allocdLengthTally = TryAllocateBlocksInSlabs(size, MAX_SEGMENTS_PER_BUFFER, slabArr, ref allocatedBlocks);

                    if (allocdLengthTally == size)
                    {
                        //We got the entire length we are looking for, so leave
                        return GetFilledBuffer(allocatedBlocks, filledWith);
                    }

                    SetSingleSlabPool(false); // Slab count will soon be incremented
                }
                else
                {
                    lock (syncSlabList)
                    {
                        slabArr = slabs.ToArray();
                    }

                    allocdLengthTally = TryAllocateBlocksInSlabs(size, MAX_SEGMENTS_PER_BUFFER, slabArr, ref allocatedBlocks);

                    if (allocdLengthTally == size)
                    {
                        //We got the entire length we are looking for, so leave
                        return GetFilledBuffer(allocatedBlocks, filledWith);
                    }
                }


                //Try to create new slab
                lock (syncNewSlab)
                {
                    //Look again for free block
                    lock (syncSlabList)
                    {
                        slabArr = slabs.ToArray();
                    }

                    allocdLengthTally += TryAllocateBlocksInSlabs(size - allocdLengthTally, MAX_SEGMENTS_PER_BUFFER - allocatedBlocks.Count, slabArr, ref allocatedBlocks);

                    if (allocdLengthTally == size)
                    {
                        //found it -- leave
                        return GetFilledBuffer(allocatedBlocks, filledWith);
                    }

                    List<IMemorySlab> newSlabList = new List<IMemorySlab>();
                    do
                    {
                        //Unable to find available free space, so create new slab
                        MemorySlab newSlab = new MemorySlab(slabSize, this);

                        IMemoryBlock allocdBlk;
                        if (slabSize > size - allocdLengthTally)
                        {
                            //Allocate remnant
                            newSlab.TryAllocate(size - allocdLengthTally, out allocdBlk);
                        }
                        else
                        {
                            //Allocate entire slab
                            newSlab.TryAllocate(slabSize, out allocdBlk);
                        }

                        newSlabList.Add(newSlab);
                        allocatedBlocks.Add(allocdBlk);
                        allocdLengthTally += allocdBlk.Length;
                    }
                    while (allocdLengthTally < size);

                    lock (syncSlabList)
                    {
                        //Add new slabs to collection
                        slabs.AddRange(newSlabList);

                        //Add extra slabs as requested in object properties
                        for (int i = 0; i < subsequentSlabs - 1; i++)
                        {
                            slabs.Add(new MemorySlab(slabSize, this));
                        }
                    }

                }

                return GetFilledBuffer(allocatedBlocks, filledWith);
            }
            catch
            {
                //OOM, Thread abort exceptions and other ugly things can happen so roll back any allocated blocks.
                //This will prevent a limbo situation where those blocks are allocated but caller is unaware and can't deallocate them.

                //NOTE: This try-catch block should not be within a lock as it calls MemorySlab.Free which takes locks,
                //and in turn calls BufferPool.TryFreeSlabs which takes other locks and can lead to a dead-lock/race condition.

                //TODO: Write rollback test.

                for (int b = 0; b < allocatedBlocks.Count; b++)
                {
                    allocatedBlocks[b].Slab.Free(allocatedBlocks[b]);
                }

                throw;
            }

        }

        internal void TryFreeSlabs()
        {
            //NOTE: This method can free a slab just before it gets allocated but that's alright because eventually
            //buffers in the 'stray' slab will be disposed and all references to the slab will no longer exist

            lock (syncSlabList)
            {
                int emptySlabsCount = 0;
                int lastemptySlab = -1;
                for (int i = 0; i < slabs.Count; i++)
                {
                    if (slabs[i].LargestFreeBlockSize == slabSize)
                    {
                        emptySlabsCount++;
                        lastemptySlab = i;
                    }
                }

                if (emptySlabsCount > InitialSlabs) //There should be at least 1+initial slabs empty slabs before one is removed
                {
                    //TODO: MULTI-SLAB: Consider freeing all free slabs that exceed the initial slabs count
                    //'cos a buffer can span several slabs and can actually free multiple slabs instantly.

                    //remove the last empty one
                    slabs.RemoveAt(lastemptySlab);

                    if (slabs.Count == 1) SetSingleSlabPool(true);
                }

            }
        }

        private static long TryAllocateBlocksInSlabs(long totalLength, int maxBlocks, IMemorySlab[] slabs, ref List<IMemoryBlock> allocatedBlocks)
        {
            allocatedBlocks = new List<IMemoryBlock>();

            long minBlockSize;
            long allocatedSizeTally = 0;

            long largest;
            long reqLength;
            IMemoryBlock allocdBlock;
            int allocdCount = 0;
            //TODO: Figure out how to do this math without involving floating point arithmetic
            minBlockSize = (long)Math.Ceiling(totalLength / (float)maxBlocks);
            do
            {
                allocdBlock = null;
                for (int i = 0; i < slabs.Length; i++)
                {
                    largest = slabs[i].LargestFreeBlockSize;
                    if (largest >= minBlockSize)
                    {
                        //Figure out what length to request for
                        reqLength = slabs[i].TryAllocate(minBlockSize, totalLength - allocatedSizeTally, out allocdBlock);

                        if (reqLength > 0)
                        {
                            allocatedBlocks.Add(allocdBlock);
                            allocatedSizeTally += reqLength;
                            allocdCount++;
                            if (allocatedSizeTally == totalLength) return allocatedSizeTally;

                            //Calculate the new minimum block size
                            //TODO: Figure out how to do this math without involving floating point arithmetic
                            minBlockSize = (long)Math.Ceiling((totalLength - allocatedSizeTally) / (float)(maxBlocks - allocdCount));

                            //Scan again from start because there is a chance the smaller minimum block size exists in previously skipped slabs
                            break;
                        }
                    }

                }
            } while (allocdBlock != null);

            return allocatedSizeTally;
        }

        private static IBuffer GetFilledBuffer(IList<IMemoryBlock> allocatedBlocks, byte[] filledWith)
        {
            IBuffer newBuffer = new ManagedBuffer(allocatedBlocks);
            if (filledWith != null) newBuffer.FillWith(filledWith);
            return newBuffer;
        }
    }
}
