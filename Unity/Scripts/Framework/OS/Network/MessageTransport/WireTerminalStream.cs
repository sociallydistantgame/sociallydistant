#nullable enable
using System;
using System.IO;

namespace OS.Network.MessageTransport
{
	public class WireTerminalStream : Stream
	{
		private WireTerminal<byte> terminal;

		public WireTerminalStream(WireTerminal<byte> terminal)
		{
			this.terminal = terminal;
		}

		/// <inheritdoc />
		public override void Flush()
		{ }

		/// <inheritdoc />
		public override int Read(byte[] buffer, int offset, int count)
		{
			if (offset < 0 || count < 0 || offset + count > buffer.Length)
				throw new IndexOutOfRangeException(nameof(offset));

			var bytesRead = 0;

			for (var i = 0; i < count; i++)
			{
				if (!terminal.TryDequeue(out byte nextByte))
					break;

				bytesRead++;
				buffer[offset + i] = nextByte;
			}
			
			return bytesRead;
		}

		/// <inheritdoc />
		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		/// <inheritdoc />
		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		/// <inheritdoc />
		public override void Write(byte[] buffer, int offset, int count)
		{
			for (var i = offset; i < offset + count; i++)
			{
				byte nextByte = buffer[i];
				terminal.Enqueue(nextByte);
			}
		}

		/// <inheritdoc />
		public override bool CanRead => true;

		/// <inheritdoc />
		public override bool CanSeek => false;

		/// <inheritdoc />
		public override bool CanWrite => true;

		/// <inheritdoc />
		public override long Length => terminal.Count;

		/// <inheritdoc />
		public override long Position
		{
			get => throw new NotSupportedException();
			set => throw new NotSupportedException();
		}
	}
}