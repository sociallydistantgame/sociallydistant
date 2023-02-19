using System;
using System.IO;
using System.Threading;

namespace UI.Terminal.SimpleTerminal.Pty
{
#pragma warning disable CS1591
    /// <summary>
    /// Represents a stream used for a pseudo-terminal.
    /// </summary>
    public class PseudoTerminal : Stream
    {
        private ThreadSafeFifoBuffer inputStream;
        private ThreadSafeFifoBuffer outputStream;

        public int CursorLeft => this.options.CursorLeft;
        public int CursorTop => this.options.CursorTop;
        public int Rows => this.options.RowCount;
        public int Columns => this.options.ColumnCount;

        /// <inheritdoc/>
        public override bool CanRead => true;

        /// <inheritdoc/>
        public override bool CanSeek => false;

        /// <inheritdoc/>
        public override bool CanWrite => true;

        /// <inheritdoc/>
        public override long Length => -1;

        /// <inheritdoc/>
        public override long Position
        {
            get => -1;

            set => throw new NotSupportedException();
        }

        private TerminalOptions options;

        private bool isMaster;

        private int lineBufferPosition = 0;
        private byte[] lineBuffer = new byte[1024];

        private PseudoTerminal(TerminalOptions ptyOptions,
            ThreadSafeFifoBuffer inputPipe,
            ThreadSafeFifoBuffer outputPipe,
            bool isMaster)
        {
            this.inputStream = inputPipe;
            this.outputStream = outputPipe;
            this.inputStream.ThrowOnTerminationRequest = true;
            this.outputStream.ThrowOnTerminationRequest = false;
            this.options = ptyOptions;
            this.isMaster = isMaster;
        }


        private void WriteOutput(byte c)
        {
            if (c == '\n' && (this.options.OFlag & PtyConstants.ONLCR) != 0) this.outputStream.WriteByte((byte)'\r');

            this.outputStream.WriteByte(c);
        }

        private void WriteInput(byte c)
        {
            if ((this.options.LFlag & PtyConstants.ICANON) != 0)
            {
                if (c == this.options.C_cc[PtyConstants.VERASE])
                {
                    if (this.lineBufferPosition > 0) this.lineBufferPosition--;

                    this.lineBuffer[this.lineBufferPosition] = 0;

                    this.WriteOutput((byte)'\b');

                    return;
                }

                if (c == this.options.C_cc[PtyConstants.VINTR])
                {
                    this.WriteOutput((byte)'^');
                    this.WriteOutput((byte)'C');
                    this.WriteOutput((byte)'\n');

                    Monitor.PulseAll(this.inputStream);
                    this.FlushLineBuffer();

                    return;
                }

                this.lineBuffer[this.lineBufferPosition++] = c;

                if ((this.options.LFlag & PtyConstants.ECHO) != 0) this.WriteOutput(c);

                if (c == (byte)'\n')
                {
                    Monitor.PulseAll(this.inputStream);
                    this.FlushLineBuffer();
                }

                return;
            }

            this.inputStream.WriteByte(c);
        }

        private void FlushLineBuffer()
        {
            this.inputStream.Write(this.lineBuffer, 0, this.lineBufferPosition);
            this.lineBufferPosition = 0;
            Monitor.PulseAll(this.inputStream);
        }

        /// <inheritdoc/>
        public override void Flush()
        {
        }

        /// <inheritdoc/>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (this.isMaster) return this.outputStream.Read(buffer, offset, count);

            return this.inputStream.Read(buffer, offset, count);
        }

        /// <inheritdoc/>
        public override long Seek(long offset, SeekOrigin origin)
        {
            return -1;
        }

        /// <inheritdoc/>
        public override void SetLength(long value)
        {
        }

        /// <inheritdoc/>
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (this.isMaster)
                for (int i = offset; i < offset + count; i++)
                    this.WriteInput(buffer[i]);
            else
                for (int i = offset; i < offset + count; i++)
                    this.WriteOutput(buffer[i]);
        }

        /// <inheritdoc/>
        public static void CreatePair(out PseudoTerminal master, out PseudoTerminal slave, TerminalOptions options)
        {
            var inputStream = new ThreadSafeFifoBuffer();
            var outputStream = new ThreadSafeFifoBuffer();

            master = new PseudoTerminal(options, inputStream, outputStream, true);
            slave = new PseudoTerminal(options, inputStream, outputStream, false);
        }
    }
}