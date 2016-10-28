namespace Gu.Persist.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;

    internal class BlockStream : Stream
    {
        public const int BlockSize = 1024;
        private readonly List<byte[]> blocks = new List<byte[]>();
        private long position;
        private int length;

        public BlockStream()
        {
        }

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => true;

        public override long Length => this.length;

        public override long Position
        {
            get { return this.position; }
            set { this.position = value; }
        }

        public override long Seek(long offset, SeekOrigin loc) => ThrowNotSupported<long>();

        public override void SetLength(long value) => ThrowNotSupported();

        public override int ReadByte() => ThrowNotSupported<int>();

        public override void WriteByte(byte value) => ThrowNotSupported();

        public override void Flush()
        {
            // nop
        }

        [System.Security.SecuritySafeCritical]
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (offset % BlockSize != 0)
            {
                throw new NotSupportedException($"Buffer size must be {BlockSize}");
            }

            var cursor = this.GetBlockAndRelativeOffset(offset);
            var block = this.blocks[cursor.Block];
            Buffer.BlockCopy(block, 0, buffer, offset, count);
            return count;
        }

        [System.Security.SecuritySafeCritical]
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (offset % BlockSize != 0)
            {
                throw new NotSupportedException($"Buffer size must be {BlockSize}");
            }

            var block = new byte[BlockSize];
            Buffer.BlockCopy(buffer, offset, block, 0, count);
            this.blocks.Add(block);
            this.length += count;
        }

        public override void Close()
        {
            this.Dispose(true);
        }

        protected override void Dispose(bool disposing)
        {
            this.blocks.Clear();
            base.Dispose(disposing);
        }

        private static void ThrowNotSupported([CallerMemberName] string caller = null)
        {
            throw new NotSupportedException(caller + " is not supported");
        }

        private static T ThrowNotSupported<T>([CallerMemberName] string caller = null)
        {
            throw new NotSupportedException(caller + " is not supported");
        }

        private BlockAndOffset GetBlockAndRelativeOffset(int offset)
        {
            return new BlockAndOffset(offset / BlockSize, offset % BlockSize);
        }

        private struct BlockAndOffset
        {
            public readonly int Block;
            public readonly int Offset;

            public BlockAndOffset(int block, int offset)
            {
                this.Block = block;
                this.Offset = offset;
            }
        }
    }
}
