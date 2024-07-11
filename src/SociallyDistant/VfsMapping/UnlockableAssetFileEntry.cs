#nullable enable
using SociallyDistant.Core.Core;
using SociallyDistant.Core.OS.FileSystems;

namespace SociallyDistant.VfsMapping
{
	public abstract class UnlockableAssetFileEntry<TAssetType> : AssetFileEntry<TAssetType>
		where TAssetType : IUnlockableAsset
	{
		/// <inheritdoc />
		protected UnlockableAssetFileEntry(IDirectoryEntry parent, TAssetType asset) : base(parent, asset)
		{
			
		}
	}
}