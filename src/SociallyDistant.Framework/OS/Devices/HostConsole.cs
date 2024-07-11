using Serilog;

namespace SociallyDistant.Core.OS.Devices;

public class HostConsole : ITextConsole
{
    public string WindowTitle { get; set; } = string.Empty;
    public bool IsInteractive { get; } = false;
    public void ClearScreen()
    {
    }

    public void WriteText(string text)
    {
        Log.Information(text);
    }

    public ConsoleInputData? ReadInput()
    {
        return null;
    }
}