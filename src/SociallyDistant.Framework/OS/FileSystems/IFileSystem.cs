#nullable enable
namespace SociallyDistant.Core.OS.FileSystems
{
	public interface IFileSystem : IMountManager
	{
		IDirectoryEntry RootDirectory { get; }
	}
}