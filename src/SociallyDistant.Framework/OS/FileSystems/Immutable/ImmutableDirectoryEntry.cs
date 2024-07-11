#nullable enable

using SociallyDistant.Core.OS.Devices;

namespace SociallyDistant.Core.OS.FileSystems.Immutable
{
	public class ImmutableDirectoryEntry : IDirectoryEntry
	{
		private bool subEntriesLocked;
		private bool filesLocked;
		private readonly List<IFileEntry> files = new List<IFileEntry>();
		private readonly List<IDirectoryEntry> subEntries = new List<IDirectoryEntry>();

		/// <inheritdoc />
		public string Name { get; }

		/// <inheritdoc />
		public IDirectoryEntry? Parent { get; }

		/// <inheritdoc />
		public IFileSystem FileSystem { get; }

		public ImmutableDirectoryEntry(IFileSystem fs, IDirectoryEntry? parent, string name)
		{
			this.FileSystem = fs;
			this.Name = name;
			this.Parent = parent;
		}
		
		/// <inheritdoc />
		public IEnumerable<IDirectoryEntry> ReadSubDirectories(IUser user)
		{
			return subEntries;
		}

		/// <inheritdoc />
		public IEnumerable<IFileEntry> ReadFileEntries(IUser user)
		{
			return files;
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

		public void SetSubEntries(IEnumerable<IDirectoryEntry> entrySource)
		{
			if (subEntriesLocked)
				throw new InvalidOperationException("Immutable directory sub-entry list is locked and can no longer be populated.");

			subEntries.Clear();
			subEntries.AddRange(entrySource);

			subEntriesLocked = true;
		}

		public void SetFileList(IEnumerable<IFileEntry> fileSource)
		{
			if (this.filesLocked)
				throw new InvalidOperationException("Immutable directory file lost is locked and can no longer be modified.");

			this.files.Clear();
			this.files.AddRange(fileSource);
			
			this.filesLocked = true;
		}
	}
}