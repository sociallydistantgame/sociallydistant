#nullable enable

using SociallyDistant.Core.OS.Devices;

namespace SociallyDistant.Core.OS.FileSystems
{
	public interface IDirectoryEntry
	{
		string Name { get; }
		IDirectoryEntry? Parent { get; }
		IFileSystem FileSystem { get; }

		IEnumerable<IDirectoryEntry> ReadSubDirectories(IUser user);
		IEnumerable<IFileEntry> ReadFileEntries(IUser user);

		bool TryDelete(IUser user);
		bool TryCreateDirectory(IUser user, string name, out IDirectoryEntry? entry);
		bool TryCreateFile(IUser user, string name, out IFileEntry? entry);
	}
}