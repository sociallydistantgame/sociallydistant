#nullable enable
using SociallyDistant.Architecture;
using SociallyDistant.Core.Core;
using SociallyDistant.Core.Modules;
using SociallyDistant.Core.OS.FileSystems;
using SociallyDistant.Player;

namespace SociallyDistant.VfsMapping
{
	public abstract class UnlockableAssetListVfsMap<TAssetType, TFileEntryType> : FileSystemAsset
		where TAssetType : IUnlockableAsset
		where TFileEntryType : UnlockableAssetFileEntry<TAssetType>
	{
		private readonly PlayerManager player = null!;

		protected UnlockableAssetListVfsMap(PlayerManager playerManager)
		{
			this.player = playerManager;
		}
		
		/// <inheritdoc />
		public sealed override IFileSystem GetFileSystem()
		{
			var assetList = new List<TAssetType>();
			foreach (TAssetType asset in Application.Instance.Context.ContentManager.GetContentOfType<TAssetType>())
			{
				assetList.Add(asset);
			}

			return new UnlockableAssetFileSystem<TAssetType, TFileEntryType>(assetList.ToArray(), player);
		}
	}
}