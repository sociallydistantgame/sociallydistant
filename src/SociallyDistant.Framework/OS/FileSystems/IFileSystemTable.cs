namespace SociallyDistant.Core.OS.FileSystems
{
	public interface IFileSystemTable
	{
		IEnumerable<IFileSystemTableEntry> Entries { get; }
	}
}