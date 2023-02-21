#nullable enable
using System;
using System.IO;
using OS.Devices;

namespace OS.FileSystems
{
	public interface IFileEntry
	{
		string Name { get; }
		IDirectoryEntry Parent { get; }

		bool TryDelete(IUser user);
		bool TryOpenRead(IUser user, out Stream? stream);
		bool TryOpenWrite(IUser user, out Stream? stream);
		bool TryExecute(ISystemProcess process, ITextConsole console, string[] arguments);
	}
}