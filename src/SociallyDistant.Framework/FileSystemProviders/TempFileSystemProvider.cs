using SociallyDistant.Core.OS.FileSystems;

namespace SociallyDistant.Core.FileSystemProviders;

public class TempFileSystemProvider : IFileSystemProvider
{
    public IFileSystem GetFileSystem()
    {
        return new InMemoryFileSystem();
    }
}