using System;
using System.IO;

namespace UI.Terminal.SimpleTerminal.Pty
{
    internal class ThreadSafeFifoBuffer : Stream
    {
        private byte[] realBuffer = new byte[1024];
        private int length;

        public override bool CanRead => true; // Yes...

        public override bool CanSeek => false;

        public override bool CanWrite => true;

        public override long Length => -1;

        public override long Position
        {
            get => this.length;
            set => throw new NotSupportedException(); // No.
        }

        public override void Flush()
        {
        }

        internal bool ThrowOnTerminationRequest { get; set; }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var bytesRead = 0;

            int c = Math.Min(this.length, count);

            if (c > 0)
            {
                Buffer.BlockCopy(this.realBuffer, 0, buffer, offset, c);

                if (this.length > c) Buffer.BlockCopy(this.realBuffer, c, this.realBuffer, 0, this.length - c);

                this.length -= c;
            }

            bytesRead = c;
            return bytesRead;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return -1; // No.
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException(); // No.
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (this.length + count >= this.realBuffer.Length)
                Array.Resize(ref this.realBuffer, this.realBuffer.Length * 2);

            Buffer.BlockCopy(buffer, offset, this.realBuffer, this.length, count);

            this.length += count;
        }
    }

#pragma warning disable CS1591
}