#nullable enable
using OS.Devices;
using Shell;

namespace UI.Shell
{
	public interface IProgramOpener
	{
		ISystemProcess OpenProgram(IProgram program, string[] arguments, ISystemProcess? parentProcess, ITextConsole? console);
	}
}