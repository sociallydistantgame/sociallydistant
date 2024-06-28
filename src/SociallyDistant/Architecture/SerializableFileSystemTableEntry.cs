#nullable enable
using SociallyDistant.Core.OS.FileSystems;

namespace SociallyDistant.Architecture
{
	[Serializable]
	public class SerializableFileSystemTableEntry : IFileSystemTableEntry
	{
		
		private string path = "/";
		
		
		private FileSystemAsset fileSystemAsset = null!;

		/// <inheritdoc />
		public string Path => path;

		/// <inheritdoc />
		public IFileSystemProvider FileSystemProvider => fileSystemAsset;
	}
}