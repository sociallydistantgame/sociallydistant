#nullable enable
using OS.Devices;

namespace UI.Shell
{
	public interface ITerminalProcessController
	{
		bool IsExecutionHalted { get; }
		
		void Setup(ISystemProcess process, ITextConsole consoleDevice);
		void Update();
	}
}