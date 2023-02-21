#nullable enable

using System;
using System.Collections.Generic;
using Architecture;
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
			var assetList = new List<TAssetType>();
			foreach (string searchDirectory in searchDirectories)
			{
				TAssetType[] assets = Resources.LoadAll<TAssetType>(searchDirectory);
				foreach (TAssetType asset in assets)
					if (!assetList.Contains(asset))
						assetList.Add(asset);
			}

			return new AssetFileSystem<TAssetType, TFileEntryType>(assetList.ToArray());
		}
	}
}