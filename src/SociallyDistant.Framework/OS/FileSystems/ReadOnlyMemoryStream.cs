#nullable enable
namespace SociallyDistant.Core.OS.FileSystems
{
	public sealed class ReadOnlyMemoryStream : Stream
	{
		private readonly MemoryStream underlyingStream;

		public ReadOnlyMemoryStream(byte[] buffer)
		{
			this.underlyingStream = new MemoryStream(buffer);
		}
		
		/// <inheritdoc />
		public override void Flush()
		{
			// stub
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
			throw new NotSupportedException();
		}

		/// <inheritdoc />
		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException();
		}

		/// <inheritdoc />
		public override bool CanRead => true;

		/// <inheritdoc />
		public override bool CanSeek => true;

		/// <inheritdoc />
		public override bool CanWrite => false;

		/// <inheritdoc />
		public override long Length => underlyingStream.Length;

		/// <inheritdoc />
		public override long Position
		{
			get => underlyingStream.Position;
			set => throw new NotSupportedException();
		}

		/// <inheritdoc />
		protected override void Dispose(bool disposing)
		{
			if (!disposing)
				return;

			underlyingStream.Dispose();
		}
	}
}