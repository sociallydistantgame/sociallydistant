#nullable enable
namespace OS.FileSystems.Host
{
	public sealed class InMemoryFileSystem : IFileSystem
	{
		private readonly MountManager mountManager;
		private readonly IDirectoryEntry root;

		public InMemoryFileSystem()
		{
			mountManager = new MountManager(this);
			root = new InMemoryDirectoryEntry(this, null, "/");
		}
		
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
	}
}