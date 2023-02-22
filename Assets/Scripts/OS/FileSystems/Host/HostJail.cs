#nullable enable

namespace OS.FileSystems.Host
{
	public class HostJail : IFileSystem
	{
		private readonly MountManager mountManager;
		private readonly string hostPath;
		private IDirectoryEntry? hostDirectoryEntry;

		public HostJail(string hostPath)
		{
			this.hostPath = hostPath;
			this.mountManager = new MountManager(this);
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
		public IDirectoryEntry RootDirectory
		{
			get
			{
				if (hostDirectoryEntry == null)
				{
					if (!System.IO.Directory.Exists(hostPath))
						System.IO.Directory.CreateDirectory(hostPath);
					hostDirectoryEntry = new HostDirectoryEntry(this, this.hostPath, null);
				}

				return hostDirectoryEntry;
			}
		}
	}
}