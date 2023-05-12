#nullable enable
using OS.FileSystems;
using OS.FileSystems.Immutable;

namespace OS.Devices
{
	public class PlayerFileSystem : IFileSystem
	{
		private readonly MountManager mountManager;
		private readonly PlayerComputer playerComputer;

		public PlayerFileSystem(PlayerComputer playerComputer)
		{
			this.playerComputer = playerComputer;
			this.mountManager = new MountManager(this);

			var entryBuilder = new ImmutableDirectoryTree(this);

			entryBuilder.AddDirectory("bin");
			entryBuilder.AddDirectory("dev");
			entryBuilder.AddDirectory("etc");
			entryBuilder.AddDirectory("sbin");
			entryBuilder.AddDirectory("home")
				.AddDirectory(playerComputer.PlayerUser.UserName);
			entryBuilder.AddDirectory("root");
			entryBuilder.AddDirectory("var")
				.AddDirectory("log");

			ImmutableDirectoryTree usrDirectory = entryBuilder.AddDirectory("usr");
			usrDirectory.AddDirectory("bin");
			ImmutableDirectoryTree usrLib = usrDirectory.AddDirectory("lib");
			usrLib.AddDirectory("exploits");
			usrLib.AddDirectory("payloads");
			usrDirectory.AddDirectory("share");

			RootDirectory = entryBuilder.Build();
		}
		
		/// <inheritdoc />
		public IDirectoryEntry RootDirectory { get; }

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
	}
}