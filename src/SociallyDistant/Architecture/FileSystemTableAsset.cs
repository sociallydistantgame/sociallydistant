#nullable enable
using SociallyDistant.Core.OS.FileSystems;

namespace SociallyDistant.Architecture
{
	public class FileSystemTableAsset : IFileSystemTable
	{
		
		private SerializableFileSystemTableEntry[] entries = Array.Empty<SerializableFileSystemTableEntry>();

		/// <inheritdoc />
		public IEnumerable<IFileSystemTableEntry> Entries
			=> entries.Where(x => !string.IsNullOrWhiteSpace(x.Path) && x.FileSystemProvider != null!);
	}
}