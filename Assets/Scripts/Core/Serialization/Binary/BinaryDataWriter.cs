#nullable enable
using System.IO;

namespace Core.Serialization.Binary
{
	public class BinaryDataWriter : IDataWriter
	{
		private readonly BinaryWriter binaryWriter;

		public BinaryDataWriter(BinaryWriter writer)
		{
			this.binaryWriter = writer;
		}

		/// <inheritdoc />
		public void Dispose()
		{
			binaryWriter.Dispose();
		}

		/// <inheritdoc />
		public void Write(sbyte value)
			=> binaryWriter.Write(value);

		/// <inheritdoc />
		public void Write(byte value)
			=> binaryWriter.Write(value);

		/// <inheritdoc />
		public void Write(short value)
			=> binaryWriter.Write(value);

		/// <inheritdoc />
		public void Write(ushort value)
			=> binaryWriter.Write(value);

		/// <inheritdoc />
		public void Write(int value)
			=> binaryWriter.Write(value);

		/// <inheritdoc />
		public void Write(uint value)
			=> binaryWriter.Write(value);

		/// <inheritdoc />
		public void Write(long value)
			=> binaryWriter.Write(value);

		/// <inheritdoc />
		public void Write(ulong value)
			=> binaryWriter.Write(value);

		/// <inheritdoc />
		public void Write(bool value)
			=> binaryWriter.Write(value);

		/// <inheritdoc />
		public void Write(float value)
			=> binaryWriter.Write(value);

		/// <inheritdoc />
		public void Write(double value)
			=> binaryWriter.Write(value);

		/// <inheritdoc />
		public void Write(decimal value)
			=> binaryWriter.Write(value);

		/// <inheritdoc />
		public void Write(char value)
			=> binaryWriter.Write(value);

		/// <inheritdoc />
		public void Write(string value)
			=> binaryWriter.Write(value);
	}
}