#nullable enable
using System;
using OS.FileSystems;
using OS.FileSystems.Immutable;

namespace Core.Scripting
{
	public sealed class HypervisorFileSystem : IFileSystem
	{
		private readonly IDirectoryEntry root;
		private readonly MountManager mountManager;
		
		/// <inheritdoc />
		public IFileSystem? GetMountedFileSystem(IDirectoryEntry mountPoint)
		{
			return mountManager.GetMountedFileSystem(mountPoint);
		}

		/// <inheritdoc />
		public void Mount(IDirectoryEntry mountPoint, IFileSystem filesystem)
		{
			mountManager.Mount(mountPoint, filesystem);
		}

		/// <inheritdoc />
		public void Unmount(IDirectoryEntry mountPoint)
		{
			mountManager.Unmount(mountPoint);
		}

		/// <inheritdoc />
		public bool IsMounted(IFileSystem fs)
		{
			return mountManager.IsMounted(fs);
		}

		/// <inheritdoc />
		public IDirectoryEntry RootDirectory => root;

		internal HypervisorFileSystem()
		{
			mountManager = new MountManager(this);
			
			var treeBuilder = new ImmutableDirectoryTree(this);
			
			this.root = treeBuilder.Build();
		}
	}
}