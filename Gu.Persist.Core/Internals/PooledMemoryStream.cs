namespace Gu.Persist.Core
{
    using System.Collections.Concurrent;
    using System.IO;

    internal class PooledMemoryStream : MemoryStream
    {
        private static readonly ConcurrentQueue<PooledMemoryStream> Pool = new ConcurrentQueue<PooledMemoryStream>();

        private PooledMemoryStream()
        {
        }

        public static PooledMemoryStream Borrow()
        {
            PooledMemoryStream stream;
            if (Pool.TryDequeue(out stream))
            {
                stream.SetLength(0);
            }

            return new PooledMemoryStream();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Pool.Enqueue(this);
            }
        }
    }
}
