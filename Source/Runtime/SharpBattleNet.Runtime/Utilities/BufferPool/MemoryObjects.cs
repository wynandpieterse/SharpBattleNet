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

    internal sealed class MemoryBlock : IMemoryBlock
    {
        readonly long startLoc, length;
        readonly IMemorySlab owner;

        internal MemoryBlock(long startLocation, long length, IMemorySlab slab)
        {
            if (startLocation < 0)
            {
                throw new ArgumentOutOfRangeException("startLocation", "StartLocation must be greater than 0");
            }

            startLoc = startLocation;

            if (length <= 0)
            {
                throw new ArgumentOutOfRangeException("length", "Length must be greater than 0");
            }

            this.length = length;
            if (slab == null) throw new ArgumentNullException("slab");
            this.owner = slab;

            //TODO: If this class is converted to a struct, consider implementing IComparer, IComparable -- first figure out what sorted dictionary uses those Comparer things for
        }

        public long StartLocation
        {
            get
            {
                return startLoc;
            }
        }

        public long EndLocation
        {
            get
            {
                return startLoc + length - 1;
            }
        }

        public long Length
        {
            get { return length; }
        }

        public IMemorySlab Slab
        {
            get { return owner; }
        }

    }

    internal sealed class MemorySlab : IMemorySlab
    {
        private readonly bool is64BitMachine;
        private readonly long slabSize;
        private readonly BufferPool pool;

        private Dictionary<long, FreeSpace> dictStartLoc = new Dictionary<long, FreeSpace>();
        private Dictionary<long, long> dictEndLoc = new Dictionary<long /* endlocation */, long /* start location*/>();
        private SortedDictionary<long, List<long>> freeBlocksList = new SortedDictionary<long /* free block length */, List<long> /* list of start locations*/>();
        private object sync = new object();

        private long largest;
        private byte[] array;

        internal MemorySlab(long size, BufferPool pool)
        {

            if (size < 1)
            {
                //Can't have zero length or -ve slabs
                throw new ArgumentOutOfRangeException("size");
            }

            //NOTE: Pool parameter is allowed to be null for testing purposes

            if (System.IntPtr.Size > 4)
            {
                is64BitMachine = true;
            }
            else
            {
                is64BitMachine = false;
            }

            // lock is unnecessary in this instance constructor
            //lock (sync)
            //{
                FreeSpace first;
                if (!dictStartLoc.TryGetValue(0, out first))
                {
                    AddFreeBlock(0, size, false);
                    this.slabSize = size;
                    this.pool = pool;
                    // GC.Collect(); //Perform Garbage Collection before creating large array -- commented out but may be useful
                    array = new byte[size];
                }
            //}
        }

        public long LargestFreeBlockSize
        {
            get { return GetLargest(); }
        }

        public long Size
        {
            get
            {
                return slabSize;
            }
        }

        public byte[] Array
        {
            get
            {
                return array;
            }
        }

        public bool TryAllocate(long length, out IMemoryBlock allocatedBlock)
        {
            return TryAllocate(length, length, out allocatedBlock) > 0;
        }

        public long TryAllocate(long minLength, long maxLength, out IMemoryBlock allocatedBlock)
        {
            if (minLength > maxLength) throw new ArgumentException("minLength is greater than maxLength", "minLength");
            if (minLength <= 0) throw new ArgumentOutOfRangeException("minLength must be greater than zero", "minLength");
            if (maxLength <= 0) throw new ArgumentOutOfRangeException("maxLength must be greater than zero", "maxLength");

            allocatedBlock = null;
            lock (sync)
            {
                if (freeBlocksList.Count == 0) return 0;

                long[] keys = new long[freeBlocksList.Count];
                freeBlocksList.Keys.CopyTo(keys, 0);

                //Leave if the largest free block cannot hold minLength
                if (keys[keys.LongLength - 1] < minLength)
                {
                    return 0;
                }

                //search freeBlocksList looking for the smallest available free block than can fit maxLength
                long length = maxLength;
                int index = System.Array.BinarySearch<long>(keys, maxLength);
                if (index < 0)
                {
                    index = ~index;
                    if (index >= keys.Length)
                    {
                        //index is set to the largest free block which can hold minLength
                        index = keys.Length - 1;

                        //length is set to the size of that free block
                        length = keys[index];
                    }
                }


                //Grab the first memoryblock in the freeBlockList innerSortedDictionary
                //There is guanranteed to be at least one in the innerList
                FreeSpace foundBlock = dictStartLoc[freeBlocksList[keys[index]][0]];

                if (foundBlock.Length == length)
                {
                    //Perfect match:

                    //Remove existing free block -- and set Largest if need be
                    RemoveFreeBlock(foundBlock, false);

                    allocatedBlock = new MemoryBlock(foundBlock.Offset, foundBlock.Length, this);
                }
                else
                {
                    //FoundBlock is larger than requested block size

                    //Shrink the existing free memory block by the new allocation
                    ShrinkFreeMemoryBlock(foundBlock, foundBlock.Length - length);

                    allocatedBlock = new MemoryBlock(foundBlock.Offset, length, this);
                }

                return length;
            }

        }

        public void Free(IMemoryBlock allocatedBlock)
        {
            //NOTE: This method can call the pool to do some cleanup (which holds locks), therefore do not call this method from within any lock
            //Or you'll get into a deadlock.

            lock (sync)
            {
                //Attempt to coalesce/merge free blocks around the allocateblock to be freed.
                long? newFreeStartLocation = null;
                long newFreeSize = 0;

                if (allocatedBlock.StartLocation > 0)
                {

                    //Check if block before this one is free

                    long startLocBefore;
                    if (dictEndLoc.TryGetValue(allocatedBlock.StartLocation - 1, out startLocBefore))
                    {
                        //Yup, so remove the free block
                        newFreeStartLocation = startLocBefore;
                        newFreeSize += (allocatedBlock.StartLocation - startLocBefore);
                        RemoveFreeBlock(dictStartLoc[startLocBefore], true);
                    }

                }

                //Include  AllocatedBlock
                if (!newFreeStartLocation.HasValue) newFreeStartLocation = allocatedBlock.StartLocation;
                newFreeSize += allocatedBlock.Length;

                if (allocatedBlock.EndLocation + 1 < Size)
                {
                    // Check if block next to (below) this one is free
                    FreeSpace blockAfter;
                    if (dictStartLoc.TryGetValue(allocatedBlock.EndLocation + 1, out blockAfter))
                    {
                        //Yup, remove the free block
                        newFreeSize += blockAfter.Length;
                        RemoveFreeBlock(blockAfter, true);
                    }
                }

                //Mark entire contiguous block as free -- and set Largest if need be:
                //The length of the AddFreeBlock call will always be longer than or equals to any of the RemoveFreeBlock
                // calls, so it's SetLargest logic will always work.
                AddFreeBlock(newFreeStartLocation.Value, newFreeSize, false);

            }

            if (GetLargest() == slabSize)
            {
                //This slab is empty. prod pool to do some cleanup
                if (pool != null)
                {
                    pool.TryFreeSlabs();
                }
            }

        }

        private void SetLargest(long value)
        {
            if (is64BitMachine)
            {
                largest = value;
                //Interlocked.Exchange(ref largest, value);
            }
            else
            {
                Interlocked.Exchange(ref largest, value);
            }

        }

        private void AddFreeBlock(long offset, long length, bool suppressSetLargest)
        {
            dictStartLoc.Add(offset, new FreeSpace(offset, length));
            dictEndLoc.Add(offset + length - 1, offset);

            List<long> innerList;
            if (!freeBlocksList.TryGetValue(length, out innerList))
            {
                innerList = new List<long>();
                innerList.Add(offset);
                freeBlocksList.Add(length, innerList);
            }
            else
            {
                int index = innerList.BinarySearch(offset);
                System.Diagnostics.Debug.Assert(index < 0); //This should always be negative as there should be no other freeblock with that offset
                index = ~index;
                innerList.Insert(index, offset);
            }

            if (!suppressSetLargest && GetLargest() < length)
            {
                SetLargest(length);
            }
        }

        
        private void RemoveFreeBlock(FreeSpace block, bool suppressSetLargest)
        {
            ShrinkOrRemoveFreeMemoryBlock(block, 0, suppressSetLargest);
        }

        private void ShrinkFreeMemoryBlock(FreeSpace block, long shrinkTo)
        {
            ShrinkOrRemoveFreeMemoryBlock(block, shrinkTo, false);
        }


        private void ShrinkOrRemoveFreeMemoryBlock(FreeSpace block, long shrinkTo, bool suppressSetLargest)
        {
            //NOTE: Do not call this method directly, instead call ShrinkFreeMemoryBlock() or RemoveFreeBlock()

            //If shrinking confirm that suppressSetLargest is false
            System.Diagnostics.Debug.Assert((shrinkTo > 0 && suppressSetLargest == false) || shrinkTo == 0);
            
            System.Diagnostics.Debug.Assert(shrinkTo <= block.Length);
            System.Diagnostics.Debug.Assert(shrinkTo >= 0 );

            dictStartLoc.Remove(block.Offset);
            dictEndLoc.Remove(block.End);

            bool calcLargest = false;
            List<long> innerList = freeBlocksList[block.Length];
            if (innerList.Count == 1)
            {
                freeBlocksList.Remove(block.Length);
                if (!suppressSetLargest && GetLargest() == block.Length)
                {
                    //The largest free block was removed, so there will be a new largest
                    calcLargest = true;
                }
            }
            else
            {
                //Find location of this block in the innerlist and remove it
                int index = innerList.BinarySearch(block.Offset);
                System.Diagnostics.Debug.Assert(index >= 0);
                innerList.RemoveAt(index);
            }

            if (shrinkTo > 0)
            {
                AddFreeBlock(block.Offset + (block.Length - shrinkTo), shrinkTo, calcLargest); 
            }

            if (calcLargest)
            {
                //Get the true largest
                if (freeBlocksList.Count == 0)
                {
                    SetLargest(0);
                }
                else
                {
                    long[] indices = new long[freeBlocksList.Count];
                    freeBlocksList.Keys.CopyTo(indices, 0);
                    SetLargest(indices[indices.LongLength - 1]);
                }
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private long GetLargest()
        {
            //This IF statement should be sufficiently complex to prevent inline optimization,
            //however the method is also marked [MethodImpl(MethodImplOptions.NoInlining)] to be extra sure

            if (is64BitMachine)
            {
                return largest;
            }
            else
            {
                return Interlocked.Read(ref largest);
            }
        }

        private struct FreeSpace
        {
            public FreeSpace(long offset, long length)
            {
                Offset = offset;
                Length = length;
            }

            /// <summary>
            /// The offset within the slab array where the free block begins
            /// </summary>
            public long Offset;

            /// <summary>
            /// The length of the free block
            /// </summary>
            public long Length;

            /// <summary>
            /// The end location of the free block
            /// </summary>
            public long End { get { return Offset + Length - 1; } }
        }
    }
}
