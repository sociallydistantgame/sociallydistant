using Core.Serialization;

namespace GameplaySystems.Networld
{
	public struct TransmissionProtocolMessage : ISerializable
	{
		public uint ConnectionId;
		public byte[] Data;
		
		/// <inheritdoc />
		public void Write(IDataWriter writer)
		{
			writer.Write(ConnectionId);
			writer.Write(Data?.Length ?? 0);

			if (Data == null)
				return;

			for (var i = 0; i < Data.Length; i++)
				writer.Write(Data[i]);
		}

		/// <inheritdoc />
		public void Read(IDataReader reader)
		{
			ConnectionId = reader.Read_uint();

			int dataLength = reader.Read_int();
			this.Data = new byte[dataLength];

			for (var i = 0; i < dataLength; i++)
				this.Data[i] = reader.Read_byte();
		}
	}
}