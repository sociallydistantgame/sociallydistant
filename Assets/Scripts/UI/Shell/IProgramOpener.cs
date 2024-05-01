#nullable enable
using OS.Devices;
using Shell;
using System.Threading.Tasks;

namespace UI.Shell
{
	public interface IProgramOpener
	{
		Task<ISystemProcess> OpenProgram(IProgram program, string[] arguments, ISystemProcess? parentProcess, ITextConsole? console);
	}
}