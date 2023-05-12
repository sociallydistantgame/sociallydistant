#nullable enable
using System;
using OS.Devices;
using OS.FileSystems;

namespace GameplaySystems.NonPlayerComputers
{
	public class NpcFileOverrider : IFileOverrider
	{
		/// <inheritdoc />
		public bool TryGetFile(IUser user, ReadOnlySpan<string> directory, string filename, out IFileEntry? entry)
		{
			entry = null;
			return false;
		}
	}
}