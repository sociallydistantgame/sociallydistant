#nullable enable
namespace SociallyDistant.Core.OS.Network
{
	public sealed class SafeMemoryStream : Stream
	{
		private readonly MemoryStream underlyingStream = new();

		private bool isDisposed;
		
		public bool IsDisposed => isDisposed;

		public byte[] ToArray()
		{
			if (isDisposed)
				return Array.Empty<byte>();
			
			return underlyingStream.ToArray();
		}
		
		/// <inheritdoc />
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				underlyingStream.Dispose();
				isDisposed = true;
			}
		}

		/// <inheritdoc />
		public override void Flush()
		{
			if (isDisposed)
				return;
			
			underlyingStream.Flush();
		}

		/// <inheritdoc />
		public override int Read(byte[] buffer, int offset, int count)
		{
			if (isDisposed)
				return 0;

			try
			{
				return underlyingStream.Read(buffer, offset, count);
			}
			catch (ObjectDisposedException)
			{
				// We lost the underlying MemoryStream, probably because another thread disposed us.
				isDisposed = true;
				return 0;
			}
		}

		/// <inheritdoc />
		public override long Seek(long offset, SeekOrigin origin)
		{
			if (isDisposed)
				return 0;

			return underlyingStream.Seek(offset, origin);
		}

		/// <inheritdoc />
		public override void SetLength(long value)
		{
			if (isDisposed)
				return;
			
			underlyingStream.SetLength(value);
		}

		/// <inheritdoc />
		public override void Write(byte[] buffer, int offset, int count)
		{
			if (isDisposed)
				return;
			
			underlyingStream.Write(buffer, offset, count);
		}

		/// <inheritdoc />
		public override bool CanRead => !isDisposed;

		/// <inheritdoc />
		public override bool CanSeek => !isDisposed;

		/// <inheritdoc />
		public override bool CanWrite => !isDisposed;

		/// <inheritdoc />
		public override long Length => isDisposed ? 0 : underlyingStream.Length;

		/// <inheritdoc />
		public override long Position
		{
			get => isDisposed ? -1 : underlyingStream.Position;
			set
			{
				if (isDisposed)
					return;

				underlyingStream.Position = value;
			}
		}
	}
}