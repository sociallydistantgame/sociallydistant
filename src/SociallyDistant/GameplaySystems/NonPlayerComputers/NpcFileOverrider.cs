#nullable enable
using SociallyDistant.Core.OS.Devices;
using SociallyDistant.Core.OS.FileSystems;

namespace SociallyDistant.GameplaySystems.NonPlayerComputers
{
	public class NpcFileOverrider : IFileOverrider
	{
		/// <inheritdoc />
		public bool TryGetFile(IUser user, ReadOnlySpan<string> directory, string filename, out IFileEntry? entry)
		{
			entry = null;
			return false;
		}

		/// <inheritdoc />
		public IEnumerable<string> EnumerateFiles(IUser ser, string[] directory)
		{
			yield break;
		}

		/// <inheritdoc />
		public void AddFile(ReadOnlySpan<string> directory, IFileEntry entry)
		{
		}

		/// <inheritdoc />
		public void RemoveFile(ReadOnlySpan<string> directory, IFileEntry entry)
		{
		}
	}
}