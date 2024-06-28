#nullable enable
using SociallyDistant.Core.OS.Devices;

namespace SociallyDistant.Core.OS.FileSystems
{
	public interface IFileOverrider
	{
		public bool TryGetFile(IUser user, ReadOnlySpan<string> directory, string filename, out IFileEntry? entry);

		public IEnumerable<string> EnumerateFiles(IUser ser, string[] directory);

		public void AddFile(ReadOnlySpan<string> directory, IFileEntry entry);
		public void RemoveFile(ReadOnlySpan<string> directory, IFileEntry entry);
	}
}