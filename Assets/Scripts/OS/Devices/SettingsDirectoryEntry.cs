using System.Collections.Generic;
using Modules;
using OS.FileSystems;

namespace OS.Devices
{
	public sealed class SettingsDirectoryEntry : IDirectoryEntry
	{
		private readonly IGameContext game;
		private readonly SettingsFileSystem filesystem;

		/// <inheritdoc />
		public string Name { get; } = "etc";

		/// <inheritdoc />
		public IDirectoryEntry? Parent { get; } = null;

		/// <inheritdoc />
		public IFileSystem FileSystem => filesystem;

		public SettingsDirectoryEntry(SettingsFileSystem settingsFileSystem, IGameContext game)
		{
			this.game = game;
			this.filesystem = settingsFileSystem;
		}
		
		/// <inheritdoc />
		public IEnumerable<IDirectoryEntry> ReadSubDirectories(IUser user)
		{
			yield break;
		}

		/// <inheritdoc />
		public IEnumerable<IFileEntry> ReadFileEntries(IUser user)
		{
			yield return new PlayerHostnameFileEntry(this, "hostname", this.filesystem.Computer, this.game);
		}

		/// <inheritdoc />
		public bool TryDelete(IUser user)
		{
			return false;
		}

		/// <inheritdoc />
		public bool TryCreateDirectory(IUser user, string name, out IDirectoryEntry? entry)
		{
			entry = null;
			return false;
		}

		/// <inheritdoc />
		public bool TryCreateFile(IUser user, string name, out IFileEntry? entry)
		{
			entry = null;
			return false;
		}
	}
}