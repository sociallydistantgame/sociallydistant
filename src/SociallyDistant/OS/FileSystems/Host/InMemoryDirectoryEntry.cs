#nullable enable
using SociallyDistant.Core.OS.Devices;
using SociallyDistant.Core.OS.FileSystems;

namespace SociallyDistant.OS.FileSystems.Host
{
	public sealed class InMemoryDirectoryEntry : IDirectoryEntry
	{
		private readonly InMemoryFileSystem fs;
		private readonly string name;
		private readonly IDirectoryEntry? parent;
		private readonly List<IDirectoryEntry> childDirectories = new List<IDirectoryEntry>();
		private readonly List<IFileEntry> files = new List<IFileEntry>();
		

		/// <inheritdoc />
		public string Name => name;

		/// <inheritdoc />
		public IDirectoryEntry? Parent => parent;

		/// <inheritdoc />
		public IFileSystem FileSystem => fs;

		public InMemoryDirectoryEntry(InMemoryFileSystem fs, IDirectoryEntry? parent, string name)
		{
			this.fs = fs;
			this.parent = parent;
			this.name = name;
		}
		
		/// <inheritdoc />
		public IEnumerable<IDirectoryEntry> ReadSubDirectories(IUser user)
		{
			foreach (IDirectoryEntry child in childDirectories)
			{
				yield return child;
			}
		}

		/// <inheritdoc />
		public IEnumerable<IFileEntry> ReadFileEntries(IUser user)
		{
			foreach (IFileEntry child in files)
			{
				yield return child;
			}
		}

		/// <inheritdoc />
		public bool TryDelete(IUser user)
		{
			return true;
		}

		/// <inheritdoc />
		public bool TryCreateDirectory(IUser user, string name, out IDirectoryEntry? entry)
		{
			entry = default;
			
			// Find a directory with the same name
			if (this.ReadSubDirectories(user).Any(x => x.Name == name))
				return false;
			
			if (this.ReadFileEntries(user).Any(x => x.Name == name))
				return false;

			entry = new InMemoryDirectoryEntry(fs, this, name);
			this.childDirectories.Add(entry);
			return true;
		}

		/// <inheritdoc />
		public bool TryCreateFile(IUser user, string name, out IFileEntry? entry)
		{
			entry = default;
			
			// Find a directory with the same name
			if (this.ReadSubDirectories(user).Any(x => x.Name == name))
				return false;
			
			if (this.ReadFileEntries(user).Any(x => x.Name == name))
				return false;

			entry = new InMemoryFileEntry(fs, this, name);
			this.files.Add(entry);
			return true;
		}
	}
}