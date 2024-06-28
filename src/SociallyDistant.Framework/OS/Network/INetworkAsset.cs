#nullable enable
using SociallyDistant.Core.ContentManagement;
using SociallyDistant.Core.Core;

namespace SociallyDistant.Core.OS.Network
{
	public interface INetworkAsset : IGameContent
	{
		Task Build(IWorldManager worldManager);
	}
}