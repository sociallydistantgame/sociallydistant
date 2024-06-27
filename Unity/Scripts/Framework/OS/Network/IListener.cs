#nullable enable
namespace OS.Network
{
	public interface IListener
	{
		ServerInfo ServerInfo { get; }

		IConnection? AcceptConnection();

		void Close();
	}
}