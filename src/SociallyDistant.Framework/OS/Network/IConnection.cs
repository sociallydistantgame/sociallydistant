#nullable enable
using SociallyDistant.Core.Core.Serialization;

namespace SociallyDistant.Core.OS.Network
{
	public interface IConnection : IDisposable
	{
		bool Connected { get; }
		
		ServerInfo ServerInfo { get; }

		bool Receive(out byte[] data);

		void Send(byte[] data);
	}

	public interface IPacketMessage
	{
		void Write(IDataWriter writer);
		void Read(IDataReader reader);
	}
}