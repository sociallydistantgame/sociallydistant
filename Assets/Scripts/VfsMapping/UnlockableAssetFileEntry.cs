#nullable enable
using Architecture;
using Core;
using OS.FileSystems;

namespace VfsMapping
{
	public abstract class UnlockableAssetFileEntry<TAssetType> : AssetFileEntry<TAssetType>
		where TAssetType : UnityEngine.Object, IUnlockableAsset
	{
		/// <inheritdoc />
		protected UnlockableAssetFileEntry(IDirectoryEntry parent, TAssetType asset) : base(parent, asset)
		{
			
		}
	}
}