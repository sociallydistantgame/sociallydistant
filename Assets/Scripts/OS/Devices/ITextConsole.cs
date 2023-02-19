#nullable enable

namespace OS.Devices
{
	public interface ITextConsole
	{
		void ClearScreen();
		void WriteText(string text);
		bool TryDequeueSubmittedInput(out string input);
	}
}