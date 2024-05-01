#nullable enable
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OS.Devices;

namespace OS.FileSystems.Host
{
	public class HostDirectoryEntry : IDirectoryEntry
	{
		private readonly string hostPath;

		/// <inheritdoc />
		public string Name => Path.GetFileName(hostPath);

		/// <inheritdoc />
		public IDirectoryEntry? Parent { get; }

		/// <inheritdoc />
		public IFileSystem FileSystem { get; }

		public HostDirectoryEntry(IFileSystem fs, string hostPath, IDirectoryEntry? parent)
		{
			this.Parent = parent;
			this.FileSystem = fs;
			this.hostPath = hostPath;
		}
		
		/// <inheritdoc />
		public IEnumerable<IDirectoryEntry> ReadSubDirectories(IUser user)
		{
			foreach (string hostSubDirectory in Directory.GetDirectories(hostPath))
			{
				yield return new HostDirectoryEntry(this.FileSystem, hostSubDirectory, this);
			}
		}

		/// <inheritdoc />
		public IEnumerable<IFileEntry> ReadFileEntries(IUser user)
		{
			foreach (string hostFile in Directory.GetFiles(this.hostPath))
			{
				yield return new HostFileEntry(this, hostFile);
			}
		}

		/// <inheritdoc />
		public bool TryDelete(IUser user)
		{
			// Read the directory entry list and file list to see if there are any child entries.
			// If there are, we can't delete.
			if (ReadSubDirectories(user).Any())
				return false;

			if (ReadFileEntries(user).Any())
				return false;
			
			// Delete the directory.
			Directory.Delete(hostPath);
			return true;
		}

		/// <inheritdoc />
		public bool TryCreateDirectory(IUser user, string name, out IDirectoryEntry? entry)
		{
			string fullPath = Path.Combine(hostPath, name);
			if (!Directory.Exists(fullPath))
				Directory.CreateDirectory(fullPath);
			
			entry = new HostDirectoryEntry(this.FileSystem, fullPath, this);
			return true;
		}

		/// <inheritdoc />
		public bool TryCreateFile(IUser user, string name, out IFileEntry? entry)
		{
			string fullPath = Path.Combine(hostPath, name);
			if (!File.Exists(fullPath))
				File.Create(fullPath).Dispose();

			entry = new HostFileEntry(this, fullPath);
			return true;
		}
	}
}