#nullable enable
namespace SociallyDistant.Core.OS.FileSystems
{
	public interface IFileSystemProvider
	{
		IFileSystem GetFileSystem();
	}
}