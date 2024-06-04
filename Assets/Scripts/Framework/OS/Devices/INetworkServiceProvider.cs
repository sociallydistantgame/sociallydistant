#nullable enable
using ContentManagement;

namespace OS.Devices
{
	public interface INetworkServiceProvider : IGameContent
	{
		public string Id { get; }

		INetworkService CreateService(ISystemProcess initProcess);
	}
}