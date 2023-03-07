#nullable enable
using Core.Serialization;

namespace OS.Network
{
	public struct Packet : ISerializable
	{
		public ushort SourcePort;
		public ushort DestinationPort;
		public uint SourceAddress;
		public uint DestinationAddress;
		public byte[] Data;
		
		/// <inheritdoc />
		public void Write(IDataWriter writer)
		{
			writer.Write(SourceAddress);
			writer.Write(DestinationAddress);
			writer.Write(SourcePort);
			writer.Write(DestinationPort);

			long length = Data?.LongLength ?? 0l;
			writer.Write(length);

			if (Data == null)
				return;

			for (var i = 0; i < length; i++)
			{
				byte b = Data[i];
				writer.Write(b);
			}
		}

		/// <inheritdoc />
		public void Read(IDataReader reader)
		{
			SourceAddress = reader.Read_uint();
			DestinationAddress = reader.Read_uint();
			SourcePort = reader.Read_ushort();
			DestinationPort = reader.Read_ushort();

			long dataLength = reader.Read_long();

			this.Data = new byte[dataLength];

			for (var i = 0; i < dataLength; i++)
			{
				Data[i] = reader.Read_byte();
			}
		}
	}
}