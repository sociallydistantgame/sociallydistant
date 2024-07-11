#nullable enable
using SociallyDistant.Core.OS.Devices;

namespace SociallyDistant.Core.Core.Scripting
{
	public interface ITerminalProcessController
	{
		bool IsExecutionHalted { get; }
		
		void Setup(ITextConsole consoleDevice);

		Task Run();
	}
}