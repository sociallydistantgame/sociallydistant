#nullable enable
namespace OS.FileSystems
{
	public interface IFileSystemProvider
	{
		IFileSystem GetFileSystem();
	}
}