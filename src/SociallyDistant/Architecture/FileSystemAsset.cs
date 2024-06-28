#nullable enable

using SociallyDistant.Core.OS.FileSystems;

namespace SociallyDistant.Architecture
{
	public abstract class FileSystemAsset : IFileSystemProvider
	{
		/// <inheritdoc />
		public abstract IFileSystem GetFileSystem();
	}
}