#nullable enable
using SociallyDistant.GameplaySystems.Hacking;
using SociallyDistant.Player;

namespace SociallyDistant.VfsMapping
{
	public class PayloadVfsMap : UnlockableAssetListVfsMap<PayloadAsset, PayloadFile>
	{
		public PayloadVfsMap(PlayerManager playerManager) : base(playerManager)
		{
		}
	}
}