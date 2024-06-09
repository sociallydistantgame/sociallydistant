using System.Collections.Generic;

namespace OS.FileSystems
{
	public interface IFileSystemTable
	{
		IEnumerable<IFileSystemTableEntry> Entries { get; }
	}
}