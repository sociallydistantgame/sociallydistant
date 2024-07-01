#nullable enable

namespace SociallyDistant.Core.OS.FileSystems
{
	public class MountManager : IMountManager
	{
		private readonly IFileSystem controller;
		private readonly Dictionary<IDirectoryEntry, IFileSystem> mountMap = new Dictionary<IDirectoryEntry, IFileSystem>();
		private readonly Dictionary<IFileSystem, IDirectoryEntry> reverseMountMap = new Dictionary<IFileSystem, IDirectoryEntry>();

		public MountManager(IFileSystem controller)
		{
			this.controller = controller;
		}

		/// <inheritdoc />
		public IFileSystem? GetMountedFileSystem(IDirectoryEntry mountPoint)
		{
			return mountMap.TryGetValue(mountPoint, out IFileSystem? fs) ? fs : null;
		}

		/// <inheritdoc />
		public void Mount(IDirectoryEntry mountPoint, IFileSystem filesystem)
		{
			if (mountMap.ContainsKey(mountPoint))
				return;
			
			mountMap.Add(mountPoint, filesystem);
			reverseMountMap.Add(filesystem, mountPoint);
		}

		/// <inheritdoc />
		public void Unmount(IDirectoryEntry mountPoint)
		{
			if (mountMap.TryGetValue(mountPoint, out IFileSystem? fs))
			{
				mountMap.Remove(mountPoint);
				reverseMountMap.Remove(fs);
			}
		}

		/// <inheritdoc />
		public bool IsMounted(IFileSystem fs)
		{
			return reverseMountMap.ContainsKey(fs)
			       || reverseMountMap.Keys.Any(x => x.IsMounted(fs));
		}
	}
}