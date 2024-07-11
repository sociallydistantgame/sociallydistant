#nullable enable

using SociallyDistant.Architecture;
using SociallyDistant.Core.ContentManagement;
using SociallyDistant.Core.Core;
using SociallyDistant.Core.Modules;
using SociallyDistant.Core.OS.FileSystems;

namespace SociallyDistant.VfsMapping
{
	public abstract class AssetListVfsMap<TAssetType, TFileEntryType> : FileSystemAsset
		where TAssetType : INamedAsset
		where TFileEntryType : AssetFileEntry<TAssetType>
	{
		
		private string[] searchDirectories = Array.Empty<string>();
		
		/// <inheritdoc />
		public sealed override IFileSystem GetFileSystem()
		{
			IContentManager contentManager = Application.Instance.Context.ContentManager;

			TAssetType[] assetList = FindAssets(contentManager).Where(Filter).ToArray();

			return new AssetFileSystem<TAssetType, TFileEntryType>(assetList.ToArray());
		}

		protected virtual IEnumerable<TAssetType> FindAssets(IContentManager contentManager)
		{
			return contentManager.GetContentOfType<TAssetType>();
		}

		protected virtual bool Filter(TAssetType asset)
		{
			return true;
		}
	}
}