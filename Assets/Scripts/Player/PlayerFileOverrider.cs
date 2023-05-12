#nullable enable
using System;
using OS.Devices;
using OS.FileSystems;

namespace Player
{
	public sealed class PlayerFileOverrider : IFileOverrider
	{
		/// <inheritdoc />
		public bool TryGetFile(IUser user, ReadOnlySpan<string> directory, string filename, out IFileEntry? entry)
		{
			entry = null;
			return false;
		}
	}
}