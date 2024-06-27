#nullable enable
using System;
using Architecture;
using Core;
using OS.FileSystems;
using OS.FileSystems.Immutable;
using Player;

namespace VfsMapping
{
	public class AssetFileSystem<TAssetType, TFileEntryType> : IFileSystem
		where TAssetType : UnityEngine.Object, INamedAsset
		where TFileEntryType : AssetFileEntry<TAssetType>
	{
		public AssetFileSystem(TAssetType[] assets)
		{
			var entryBuilder = new ImmutableDirectoryTree(this);

			foreach (TAssetType asset in assets)
			{
				entryBuilder.AddFileRequest(x =>
				{
					IFileEntry fileEntry = (IFileEntry) Activator.CreateInstance(typeof(TFileEntryType), new object[] { x, asset });
					return fileEntry;
				});
			}

			this.RootDirectory = entryBuilder.Build();
		}

		/// <inheritdoc />
		public IFileSystem? GetMountedFileSystem(IDirectoryEntry mountPoint)
		{
			return null;
		}

		/// <inheritdoc />
		public void Mount(IDirectoryEntry mountPoint, IFileSystem filesystem)
		{
			throw new System.NotImplementedException();
		}

		/// <inheritdoc />
		public void Unmount(IDirectoryEntry mountPoint)
		{
			throw new System.NotImplementedException();
		}

		/// <inheritdoc />
		public bool IsMounted(IFileSystem fs)
		{
			return false;
		}

		/// <inheritdoc />
		public IDirectoryEntry RootDirectory { get; }
	}

	public sealed class UnlockableAssetFileSystem<TAssetType, TFileEntryType> : IFileSystem
		where TAssetType : UnityEngine.Object, IUnlockableAsset
		where TFileEntryType : UnlockableAssetFileEntry<TAssetType>
	{
		public UnlockableAssetFileSystem(TAssetType[] assets, PlayerInstanceHolder player)
		{
			RootDirectory = new UnlockableAssetDirectory<TAssetType, TFileEntryType>(this, assets, player);
		}

		/// <inheritdoc />
		public IFileSystem? GetMountedFileSystem(IDirectoryEntry mountPoint)
		{
			return null;
		}

		/// <inheritdoc />
		public void Mount(IDirectoryEntry mountPoint, IFileSystem filesystem)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public void Unmount(IDirectoryEntry mountPoint)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public bool IsMounted(IFileSystem fs)
		{
			return false;
		}

		/// <inheritdoc />
		public IDirectoryEntry RootDirectory { get; }
	}
}