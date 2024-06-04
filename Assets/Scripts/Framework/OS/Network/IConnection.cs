#nullable enable
using System;
using System.IO;
using Core.Serialization;

namespace OS.Network
{
	public interface IConnection
	{
		bool Connected { get; }
		
		ServerInfo ServerInfo { get; }

		bool Receive(out byte[] data);

		void Send(byte[] data);
	}

	public sealed class SimulatedNetworkStream : Stream
	{
		private const int MaximumWriteBufferLength = 4096;
		private readonly IConnection connection;
		private readonly MemoryStream writeBuffer = new();
		private readonly MemoryStream readBuffer = new();


		public bool AutoFlush { get; set; } = true;

		public SimulatedNetworkStream(IConnection connection)
		{
			this.connection = connection;
		}

		/// <inheritdoc />
		protected override void Dispose(bool disposing)
		{
			if (!disposing)
				return;

			Flush();
			writeBuffer.Dispose();
			readBuffer.Dispose();

			// TODO: No way to disconnect in the simulation?
		}

		/// <inheritdoc />
		public override void Flush()
		{
			byte[] data = writeBuffer.ToArray();
			
			writeBuffer.SetLength(0);
			writeBuffer.Seek(0, SeekOrigin.Begin);

			connection.Send(data);
		}

		/// <inheritdoc />
		public override int Read(byte[] buffer, int offset, int count)
		{
			var readCount = 0;
			while ((readCount += readBuffer.Read(buffer, offset + readCount, count - readCount)) < count)
			{
				if (connection.Receive(out byte[] data))
				{
					long currentPos = readBuffer.Position;
					readBuffer.Seek(0, SeekOrigin.End);
					long seekBase = readBuffer.Position - currentPos;
					long seekAmount = seekBase + data.Length;
					
					readBuffer.Write(data, 0, data.Length);
					readBuffer.Seek(-seekAmount, SeekOrigin.Current);
				}

				if (!connection.Connected)
					break;
			}

			return readCount;
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
			writeBuffer.Write(buffer, offset, count);
			
			if (AutoFlush || writeBuffer.Length==MaximumWriteBufferLength)
				Flush();
		}

		/// <inheritdoc />
		public override bool CanRead => true;

		/// <inheritdoc />
		public override bool CanSeek => false;

		/// <inheritdoc />
		public override bool CanWrite => true;

		/// <inheritdoc />
		public override long Length => -1;

		/// <inheritdoc />
		public override long Position
		{
			get => -1;
			set => throw new NotSupportedException();
		}
	}

	public interface IPacketMessage
	{
		void Write(IDataWriter writer);
		void Read(IDataReader reader);
	}
}