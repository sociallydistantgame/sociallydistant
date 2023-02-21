#nullable enable
using OS.Devices;

namespace UI.Shell
{
	public interface IProgramOpener<TProgramUserInterface>
	{
		ISystemProcess OpenProgram(IProgram<TProgramUserInterface> program, string[] arguments, ISystemProcess? parentProcess, ITextConsole? console);
	}
}