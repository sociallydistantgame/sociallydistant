#nullable enable
using ContentManagement;
using Core;
using System.Threading.Tasks;

namespace OS.Network
{
	public interface INetworkAsset : IGameContent
	{
		Task Build(IWorldManager worldManager);
	}
}