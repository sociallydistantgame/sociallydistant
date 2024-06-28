#nullable enable
using SociallyDistant.Core.Core.Serialization;

namespace SociallyDistant.Core.OS.Network
{
	public struct Packet : ISerializable
	{
		private byte packetType;
		public ushort SourcePort;
		public ushort DestinationPort;
		public uint SourceAddress;
		public uint DestinationAddress;
		public byte[] Data;
		
		public PacketType PacketType
		{
			get => (PacketType) this.packetType;
			set => this.packetType = (byte) value;
		}

		public void SwapSourceAndDestination()
		{
			(SourcePort, DestinationPort) = (DestinationPort, SourcePort);
			(SourceAddress, DestinationAddress) = (DestinationAddress, SourceAddress);
		}
		
		/// <inheritdoc />
		public void Write(IDataWriter writer)
		{
			writer.Write(SourceAddress);
			writer.Write(DestinationAddress);
			writer.Write(SourcePort);
			writer.Write(DestinationPort);

			long length = Data?.LongLength ?? 0;
			writer.Write(length);

			writer.Write(packetType);
			
			if (Data == null)
				return;

			for (var i = 0; i < length; i++)
			{
				byte b = Data[i];
				writer.Write(b);
			}
		}

		public Packet Clone()
		{
			var cloned = new Packet();

			cloned.packetType = packetType;
			cloned.SourcePort = SourcePort;
			cloned.DestinationPort = DestinationPort;
			cloned.SourceAddress = SourceAddress;
			cloned.DestinationAddress = DestinationAddress;
			cloned.Data = new byte[this.Data?.Length ?? 0];

			if (cloned.Data.Length > 0 && this.Data != null)
			{
				Array.Copy(this.Data, 0, cloned.Data, 0, cloned.Data.Length);
			}
			
			
			return cloned;
		}
		
		/// <inheritdoc />
		public void Read(IDataReader reader)
		{
			SourceAddress = reader.Read_uint();
			DestinationAddress = reader.Read_uint();
			SourcePort = reader.Read_ushort();
			DestinationPort = reader.Read_ushort();

			long dataLength = reader.Read_long();

			packetType =  reader.Read_byte();

			this.Data = new byte[dataLength];

			for (var i = 0; i < dataLength; i++)
			{
				Data[i] = reader.Read_byte();
			}
		}
	}
}