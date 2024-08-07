﻿#nullable enable
using SociallyDistant.Core.OS.Devices;

namespace SociallyDistant.Core.OS.FileSystems
{
	public interface IFileEntry
	{
		string Name { get; }
		IDirectoryEntry Parent { get; }

		bool CanExecute { get; }
		
		bool TryDelete(IUser user);
		bool TryOpenRead(IUser user, out Stream? stream);
		bool TryOpenWrite(IUser user, out Stream? stream);
		bool TryOpenWriteAppend(IUser user, out Stream? stream);
		
		Task<bool> TryExecute(ISystemProcess process, ITextConsole console, string[] arguments);
	}
}