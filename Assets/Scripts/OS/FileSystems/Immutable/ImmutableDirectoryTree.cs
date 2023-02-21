#nullable enable

using System.Collections.Generic;
using System.Linq;

namespace OS.FileSystems.Immutable
{
	public class ImmutableDirectoryTree
	{
		private readonly IFileSystem filesystem;
		private readonly List<ImmutableDirectoryTree> children = new List<ImmutableDirectoryTree>();

		public string Name { get; set; } = string.Empty;

		public ImmutableDirectoryTree(IFileSystem fs)
		{
			this.filesystem = fs;
		}

		public ImmutableDirectoryTree AddDirectory(string name)
		{
			ImmutableDirectoryTree? existing = children.FirstOrDefault(x => x.Name == name);
			if (existing != null)
				return existing;

			existing = new ImmutableDirectoryTree(this.filesystem);
			existing.Name = name;
			this.children.Add(existing);

			return existing;
		}
		
		public IDirectoryEntry Build()
		{
			return BuildInternal(null);
		}

		private ImmutableDirectoryEntry BuildInternal(IDirectoryEntry? parent)
		{
			var entry = new ImmutableDirectoryEntry(this.filesystem, parent, this.Name);

			List<IDirectoryEntry> subEntryList = children.Select(child => child.BuildInternal(entry)).Cast<IDirectoryEntry>().ToList();

			entry.SetSubEntries(subEntryList);

			// TODO: File support
			entry.SetFileList(Enumerable.Empty<IFileEntry>());

			return entry;
		}
	}
}