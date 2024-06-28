#nullable enable
namespace SociallyDistant.Core.OS.Network
{
	public enum PacketType : byte
	{
		Ping,
		Pong,
		Connect,
		ConnectAccept,
		Disconnect,
		Transmission,
		Refusal,
		IcmpPing,
		IcmpAck,
		IcmpReject,
		Void
	}
}