#nullable enable

namespace OS.Devices
{
	public interface ITextConsole
	{
		bool IsInteractive { get; }
		
		void ClearScreen();
		void WriteText(string text);
		ConsoleInputData? ReadInput();
	}
}