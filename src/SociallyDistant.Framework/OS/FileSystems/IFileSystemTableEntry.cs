namespace SociallyDistant.Core.OS.FileSystems
{
	public interface IFileSystemTableEntry
	{
		string Path { get; }
		IFileSystemProvider FileSystemProvider { get; }
	}
}