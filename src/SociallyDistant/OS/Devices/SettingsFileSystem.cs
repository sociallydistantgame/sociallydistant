using SociallyDistant.Core.Modules;
using SociallyDistant.Core.OS.Devices;
using SociallyDistant.Core.OS.FileSystems;
using SociallyDistant.OS.FileSystems;

namespace SociallyDistant.OS.Devices
{
	public sealed class SettingsFileSystem : IFileSystem
	{
		private readonly SettingsDirectoryEntry settingsDirectoryEntry;
		private readonly PlayerComputer playerComputer;
		private readonly MountManager mountManager;

		public IComputer Computer => playerComputer;
		
		public SettingsFileSystem(PlayerComputer computer, IGameContext gameContext)
		{
			this.playerComputer = computer;
			this.mountManager = new MountManager(this);
			this.settingsDirectoryEntry = new SettingsDirectoryEntry(this, gameContext);
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
		public IDirectoryEntry RootDirectory => settingsDirectoryEntry;
	}
}