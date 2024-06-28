#nullable enable
using SociallyDistant.Core.ContentManagement;

namespace SociallyDistant.Core.OS.Devices
{
	public interface INetworkServiceProvider : IGameContent
	{
		public string Id { get; }

		INetworkService CreateService(ISystemProcess initProcess);
	}
}