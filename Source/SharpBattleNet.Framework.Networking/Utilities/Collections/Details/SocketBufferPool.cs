namespace SharpBattleNet.Framework.Networking.Utilities.Collections.Details
{
    using System;
    using SharpBattleNet.Framework.External.BufferPool;

    internal sealed class SocketBufferPool : BufferPool, ISocketBufferPool
    {
        public SocketBufferPool(long slabSize, int initialSlabs, int subsequentSlabs)
            : base(slabSize, initialSlabs, subsequentSlabs)
        {
            return;
        }
    }
}
