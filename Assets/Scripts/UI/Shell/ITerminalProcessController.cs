#nullable enable
using System.Threading.Tasks;
using OS.Devices;

namespace UI.Shell
{
	public interface ITerminalProcessController
	{
		bool IsExecutionHalted { get; }
		
		void Setup(ITextConsole consoleDevice);

		Task Run();
	}
}