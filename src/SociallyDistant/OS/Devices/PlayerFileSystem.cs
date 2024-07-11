#nullable enable
using SociallyDistant.Core.OS.Devices;
using SociallyDistant.Core.OS.FileSystems;
using SociallyDistant.Core.OS.FileSystems.Host;
using SociallyDistant.Core.OS.FileSystems.Immutable;
using SociallyDistant.OS.FileSystems;

namespace SociallyDistant.OS.Devices
{
	internal class HomeEnumerator : IDirectoryEntry
	{
		private readonly string      baseDirectoryOnHost;
		private readonly IComputer   computer;
		private readonly IFileSystem filesystem;

		public HomeEnumerator(IFileSystem filesystem, IComputer computer, string baseDirectoryOnHost)
		{
			this.filesystem = filesystem;
			this.computer = computer;
			this.baseDirectoryOnHost = baseDirectoryOnHost;
		}

		public string Name => "home";
		public IDirectoryEntry? Parent { get; } = null;
		public IFileSystem FileSystem => filesystem;
		public IEnumerable<IDirectoryEntry> ReadSubDirectories(IUser user)
		{
			if (!Directory.Exists(baseDirectoryOnHost))
				Directory.CreateDirectory(baseDirectoryOnHost);
			
			string userPath = Path.Combine(baseDirectoryOnHost, user.UserName);

			if (!Directory.Exists(userPath))
				Directory.CreateDirectory(userPath);

			foreach (string directory in Directory.EnumerateDirectories(baseDirectoryOnHost))
			{
				yield return new HostDirectoryEntry(filesystem, directory, this);
			}
		}

		public IEnumerable<IFileEntry> ReadFileEntries(IUser user)
		{
			if (!Directory.Exists(baseDirectoryOnHost))
				Directory.CreateDirectory(baseDirectoryOnHost);
			
			foreach (string directory in Directory.EnumerateFiles(baseDirectoryOnHost))
			{
				yield return new HostFileEntry(this, directory);
			}
		}

		public bool TryDelete(IUser user)
		{
			return false;
		}

		public bool TryCreateDirectory(IUser user, string name, out IDirectoryEntry? entry)
		{
			entry = null;
			return false;
		}

		public bool TryCreateFile(IUser user, string name, out IFileEntry? entry)
		{
			entry = null;
			return false;
		}
	}
	
	public sealed class HomeFileSystem : IFileSystem
	{
		private readonly MountManager   mountManager;
		private readonly HomeEnumerator enumerator;

		public HomeFileSystem(IComputer computer, string baseDirectoryOnHost)
		{
			this.enumerator = new HomeEnumerator(this, computer, baseDirectoryOnHost);
			this.mountManager = new MountManager(this);
		}

		public IFileSystem? GetMountedFileSystem(IDirectoryEntry mountPoint)
		{
			return mountManager.GetMountedFileSystem(mountPoint);
		}

		public void Mount(IDirectoryEntry mountPoint, IFileSystem filesystem)
		{
			mountManager.Mount(mountPoint, filesystem);
		}

		public void Unmount(IDirectoryEntry mountPoint)
		{
			mountManager.Unmount(mountPoint);
		}

		public bool IsMounted(IFileSystem fs)
		{
			return mountManager.IsMounted(fs);
		}

		public IDirectoryEntry RootDirectory => enumerator;
	}
	
	public class RootFileSystem : IFileSystem
	{
		private readonly MountManager mountManager;
		private readonly IComputer playerComputer;

		internal RootFileSystem(SociallyDistantGame game, IComputer computer, bool isPlayer)
		{
			this.playerComputer = computer;
			this.mountManager = new MountManager(this);

			var entryBuilder = new ImmutableDirectoryTree(this);

			entryBuilder.AddDirectory("bin");
			entryBuilder.AddDirectory("dev");
			entryBuilder.AddDirectory("etc");
			entryBuilder.AddDirectory("tmp");
			entryBuilder.AddDirectory("sbin");
			entryBuilder.AddDirectory("home");
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