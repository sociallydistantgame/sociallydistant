#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

namespace OS.FileSystems.Immutable
{
	public class ImmutableDirectoryTree
	{
		private readonly IFileSystem filesystem;
		private readonly List<ImmutableDirectoryTree> children = new List<ImmutableDirectoryTree>();
		private readonly List<Unity.Plastic.Newtonsoft.Json.Serialization.Func<IDirectoryEntry, IFileEntry>> fileRequest = new List<Unity.Plastic.Newtonsoft.Json.Serialization.Func<IDirectoryEntry, IFileEntry>>();

		public string Name { get; set; } = string.Empty;

		public ImmutableDirectoryTree(IFileSystem fs)
		{
			this.filesystem = fs;
		}

		public void AddFileRequest(Unity.Plastic.Newtonsoft.Json.Serialization.Func<IDirectoryEntry, IFileEntry> creationFunction)
		{
			this.fileRequest.Add(creationFunction);
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

			var fileEntries = new List<IFileEntry>();
			foreach (Unity.Plastic.Newtonsoft.Json.Serialization.Func<IDirectoryEntry, IFileEntry>? creationFunction in fileRequest)
			{
				// Call the creation function to create the file entry
				IFileEntry fileEntry = creationFunction(entry);
				
				// Check the file is a parent of the entry!!!
				if (fileEntry.Parent != entry)
					throw new InvalidOperationException("A file entry was created for an immutable directory's file list, but the file isn't a child of the directory!");
				fileEntries.Add(fileEntry);
			}
			
			entry.SetFileList(fileEntries);

			return entry;
		}
	}
}