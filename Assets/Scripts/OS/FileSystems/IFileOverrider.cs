#nullable enable
using System;
using OS.Devices;

namespace OS.FileSystems
{
	public interface IFileOverrider
	{
		public bool TryGetFile(IUser user, ReadOnlySpan<string> directory, string filename, out IFileEntry? entry);
	}
}