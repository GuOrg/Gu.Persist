namespace Gu.Persist.Core
{
    using System;
    using System.Collections.Concurrent;
    using System.IO;

    internal class PooledMemoryStream : Stream, IPooledStream
    {
        private static readonly ConcurrentQueue<MemoryStream> Pool = new ConcurrentQueue<MemoryStream>();
        private readonly MemoryStream inner;

        private bool disposed;

        private PooledMemoryStream(MemoryStream inner)
        {
            this.inner = inner;
        }

        public override bool CanRead => !this.disposed;

        public override bool CanSeek => !this.disposed;

        public override bool CanWrite => !this.disposed;

        public override long Length => this.inner.Length;

        public override long Position
        {
            get { return this.inner.Position; }
            set { this.inner.Position = value; }
        }

        public static PooledMemoryStream Borrow()
        {
            MemoryStream stream;
            if (Pool.TryDequeue(out stream))
            {
                return new PooledMemoryStream(stream);
            }

            return new PooledMemoryStream(new MemoryStream());
        }

        public override void Flush()
        {
            // nop
        }

        public override long Seek(long offset, SeekOrigin origin) => this.inner.Seek(offset, origin);

        public override void SetLength(long value) => this.inner.SetLength(value);

        public override int Read(byte[] buffer, int offset, int count)
        {
            this.CheckDisposed();
            return this.inner.Read(buffer, offset, count);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            this.CheckDisposed();
            this.inner.Write(buffer, offset, count);
        }

        public byte[] GetBuffer() => this.inner.GetBuffer();

        protected override void Dispose(bool disposing)
        {
            if (disposing && !this.disposed)
            {
                this.inner.SetLength(0);

                Pool.Enqueue(this.inner);
            }

            this.disposed = true;
            base.Dispose(disposing);
        }

        private void CheckDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }
    }
}
