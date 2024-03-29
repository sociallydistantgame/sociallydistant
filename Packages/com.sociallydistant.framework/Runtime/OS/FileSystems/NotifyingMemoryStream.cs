#nullable enable
using System;
using System.IO;

namespace OS.FileSystems
{
	public sealed class NotifyingMemoryStream : Stream
	{
		private readonly MemoryStream underlyingStream;
		private Action<byte[]> onCloseAction;

		public NotifyingMemoryStream(Action<byte[]> notifyCallback) : this(Array.Empty<byte>(), notifyCallback)
		{
			
		}
		
		public NotifyingMemoryStream(byte[] buffer, Action<byte[]> notifyCallback)
		{
			underlyingStream = new MemoryStream();
			this.onCloseAction = notifyCallback;

			Write(buffer, 0, buffer.Length);
			this.Position = 0;
		}
		
		/// <inheritdoc />
		public override void Flush()
		{
			underlyingStream.Flush();
		}

		/// <inheritdoc />
		public override int Read(byte[] buffer, int offset, int count)
		{
			return underlyingStream.Read(buffer, offset, count);
		}

		/// <inheritdoc />
		public override long Seek(long offset, SeekOrigin origin)
		{
			return underlyingStream.Seek(offset, origin);
		}

		/// <inheritdoc />
		public override void SetLength(long value)
		{
			underlyingStream.SetLength(value);
		}

		/// <inheritdoc />
		public override void Write(byte[] buffer, int offset, int count)
		{
			underlyingStream.Write(buffer, offset, count);
		}

		/// <inheritdoc />
		public override bool CanRead => true;

		/// <inheritdoc />
		public override bool CanSeek => true;

		/// <inheritdoc />
		public override bool CanWrite => true;

		/// <inheritdoc />
		public override long Length => underlyingStream.Length;

		/// <inheritdoc />
		public override long Position
		{
			get => underlyingStream.Position;
			set => underlyingStream.Position = value;
		}
		
		/// <inheritdoc />
		protected override void Dispose(bool disposing)
		{
			if (!disposing)
				return;
			
			// Allocation in a cleanup method. Hahaha. Microsoft hates me for this.
			// Unfortunately for them, I don't work for them and I'm just the right amount
			// of smashed to not give a flying fuck about coding standards or guidelines.
			// - Ritchie
			this.onCloseAction?.Invoke(this.underlyingStream.ToArray());
			this.underlyingStream.Dispose();
			
			// but I'll at least deref the delegate
			this.onCloseAction = null;
		}
	}
}