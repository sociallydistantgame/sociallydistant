#nullable enable
namespace OS.Network
{
	public interface IConnection
	{
		bool Connected { get; }
		
		ServerInfo ServerInfo { get; }

		bool Receive(out byte[] data);

		void Send(byte[] data);
	}
}