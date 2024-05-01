#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Architecture;
using ContentManagement;
using Core;
using GamePlatform;
using OS.FileSystems;
using UnityEngine;

namespace VfsMapping
{
	public abstract class AssetListVfsMap<TAssetType, TFileEntryType> : FileSystemAsset
		where TAssetType : UnityEngine.Object, INamedAsset
		where TFileEntryType : AssetFileEntry<TAssetType>
	{
		[SerializeField]
		private string[] searchDirectories = Array.Empty<string>();
		
		/// <inheritdoc />
		public sealed override IFileSystem GetFileSystem()
		{
			IContentManager contentManager = GameManager.Instance.ContentManager;

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