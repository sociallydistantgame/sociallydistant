#nullable enable
using Player;

namespace Architecture
{
	public interface IUnlockableAsset : INamedAsset
	{
		bool IsUnlocked(PlayerInstanceHolder player);
		bool CanUnlock(PlayerInstanceHolder player);
		bool Unlock(PlayerInstanceHolder player);
	}
}