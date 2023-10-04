#nullable enable

using System;
using System.IO;

namespace Core.Serialization.Binary
{
	public class BinaryDataReader : IDataReader
	{
		private readonly BinaryReader binaryReader;

		public BinaryDataReader(BinaryReader reader)
		{
			this.binaryReader = reader;
		}

		/// <inheritdoc />
		public void Dispose()
		{
			binaryReader.Dispose();
		}

		/// <inheritdoc />
		public sbyte Read_sbyte()
		{
			return binaryReader.ReadSByte();
		}

		/// <inheritdoc />
		public byte Read_byte()
		{
			return binaryReader.ReadByte();
		}

		/// <inheritdoc />
		public short Read_short()
		{
			return binaryReader.ReadInt16();
		}

		/// <inheritdoc />
		public ushort Read_ushort()
		{
			return binaryReader.ReadUInt16();
		}

		/// <inheritdoc />
		public int Read_int()
		{
			return binaryReader.ReadInt32();
		}

		/// <inheritdoc />
		public uint Read_uint()
		{
			return binaryReader.ReadUInt32();
		}

		/// <inheritdoc />
		public long Read_long()
		{
			return binaryReader.ReadInt64();
		}

		/// <inheritdoc />
		public ulong Read_ulong()
		{
			return binaryReader.ReadUInt64();
		}

		/// <inheritdoc />
		public bool Read_bool()
		{
			return binaryReader.ReadBoolean();
		}

		/// <inheritdoc />
		public float Read_float()
		{
			return binaryReader.ReadSingle();
		}

		/// <inheritdoc />
		public double Read_double()
		{
			return binaryReader.ReadDouble();
		}

		/// <inheritdoc />
		public decimal Read_decimal()
		{
			return binaryReader.ReadDecimal();
		}

		/// <inheritdoc />
		public char Read_char()
		{
			return binaryReader.ReadChar();
		}

		/// <inheritdoc />
		public string Read_string()
		{
			return binaryReader.ReadString();
		}

		/// <inheritdoc />
		public DateTime ReadDateTime()
		{
			return DateTime.FromBinary(Read_long());
		}
	}
}