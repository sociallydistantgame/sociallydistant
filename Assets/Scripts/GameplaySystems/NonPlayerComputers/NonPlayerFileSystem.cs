#nullable enable
using Core;
using Core.DataManagement;
using OS.FileSystems;
using OS.FileSystems.Immutable;

namespace GameplaySystems.NonPlayerComputers
{
	public sealed class NonPlayerFileSystem : IFileSystem
	{
		private MountManager mountManager;
		
		public NonPlayerFileSystem(NonPlayerComputer computer, ObjectId computerId, WorldManager world)
		{
			mountManager = new MountManager(this);
			
			var entryBuilder = new ImmutableDirectoryTree(this);

			entryBuilder.AddDirectory("bin");
			entryBuilder.AddDirectory("dev");
			entryBuilder.AddDirectory("etc");
			entryBuilder.AddDirectory("sbin");
			entryBuilder.AddDirectory("home");
			entryBuilder.AddDirectory("root");
			entryBuilder.AddDirectory("var")
				.AddDirectory("log");

			ImmutableDirectoryTree usrDirectory = entryBuilder.AddDirectory("usr");
			usrDirectory.AddDirectory("bin");
			usrDirectory.AddDirectory("lib");
			usrDirectory.AddDirectory("share");

			RootDirectory = entryBuilder.Build();
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
		public IDirectoryEntry RootDirectory { get; }
	}
}