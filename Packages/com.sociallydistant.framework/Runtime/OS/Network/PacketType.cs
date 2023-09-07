#nullable enable
namespace OS.Network
{
	public enum PacketType : byte
	{
		Ping,
		Pong,
		Connect,
		ConnectAccept,
		Disconnect,
		Transmission,
		Refusal
	}
}