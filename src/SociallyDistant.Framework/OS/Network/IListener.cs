#nullable enable
namespace SociallyDistant.Core.OS.Network
{
	public interface IListener
	{
		ServerInfo ServerInfo { get; }

		IConnection? AcceptConnection();

		void Close();
	}
}