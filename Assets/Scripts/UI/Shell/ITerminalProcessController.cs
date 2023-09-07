#nullable enable
using System.Threading.Tasks;
using OS.Devices;

namespace UI.Shell
{
	public interface ITerminalProcessController
	{
		bool IsExecutionHalted { get; }
		
		void Setup(ISystemProcess process, ITextConsole consoleDevice);

		Task Run();
	}
}