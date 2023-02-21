namespace OS.FileSystems
{
	public interface IFileSystemTableEntry
	{
		string Path { get; }
		IFileSystemProvider FileSystemProvider { get; }
	}
}