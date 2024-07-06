#nullable enable
using SociallyDistant.Core.OS.FileSystems;

namespace SociallyDistant.Architecture
{
	public sealed class FileSystemTableEntry : IFileSystemTableEntry
	{
		public string Path { get; }
		public IFileSystemProvider FileSystemProvider { get; }

		public FileSystemTableEntry(string path, IFileSystemProvider provider)
		{
			Path = path;
			FileSystemProvider = provider;
		}
	}
    
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