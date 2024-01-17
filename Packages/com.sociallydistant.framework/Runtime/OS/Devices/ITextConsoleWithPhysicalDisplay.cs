#nullable enable
namespace OS.Devices
{
	public interface ITextConsoleWithPhysicalDisplay : ITextConsole
	{
		int CursorLeft { get; }
		int CursorTop { get; }
		int Width { get; }
		int Height { get; }
	}
}