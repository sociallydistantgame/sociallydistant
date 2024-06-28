#nullable enable
namespace SociallyDistant.Core.OS.Devices
{
	public interface ITextConsoleWithPhysicalDisplay : ITextConsole
	{
		int CursorLeft { get; }
		int CursorTop { get; }
		int Width { get; }
		int Height { get; }
	}
}