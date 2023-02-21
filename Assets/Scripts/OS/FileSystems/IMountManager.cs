#nullable enable
namespace OS.FileSystems
{
	public interface IMountManager
	{
		IFileSystem? GetMountedFileSystem(IDirectoryEntry mountPoint);

		void Mount(IDirectoryEntry mountPoint, IFileSystem filesystem);
		void Unmount(IDirectoryEntry mountPoint);

		bool IsMounted(IFileSystem fs);
	}
}