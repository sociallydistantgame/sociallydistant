#nullable enable
using System;
using System.IO;
using Architecture;
using Core;
using OS.Devices;
using OS.FileSystems;

namespace VfsMapping
{
	public abstract class AssetFileEntry<TAssetType> : IFileEntry
		where TAssetType : UnityEngine.Object, INamedAsset
	{
		private readonly TAssetType asset;

		/// <inheritdoc />
		public string Name { get; }

		/// <inheritdoc />
		public abstract bool CanExecute { get; }
		
		protected TAssetType Asset => asset;
		
		/// <inheritdoc />
		public IDirectoryEntry Parent { get; }

		public AssetFileEntry(IDirectoryEntry parent, TAssetType asset)
		{
			this.asset = asset;
			this.Parent = parent;
			this.Name = asset.Name;
		}

		/// <inheritdoc />
		public bool TryDelete(IUser user)
		{
			return false;
		}

		/// <inheritdoc />
		public bool TryOpenRead(IUser user, out Stream? stream)
		{
			stream = null;
			return false;
		}

		/// <inheritdoc />
		public bool TryOpenWrite(IUser user, out Stream? stream)
		{
			stream = null;
			return false;
		}

		/// <inheritdoc />
		public bool TryOpenWriteAppend(IUser user, out Stream? stream)
		{
			stream = null;
			return false;
		}

		/// <inheritdoc />
		public virtual bool TryExecute(ISystemProcess process, ITextConsole console, string[] arguments)
		{
			return false;
		}
	}
}