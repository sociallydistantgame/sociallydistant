#nullable enable
namespace OS.FileSystems
{
	public interface IFileSystem : IMountManager
	{
		IDirectoryEntry RootDirectory { get; }
	}
}