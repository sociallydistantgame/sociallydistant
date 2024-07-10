using Microsoft.Xna.Framework;

namespace SociallyDistant.Core.UI.Terminal;

public sealed class TerminalColorPalette
{
    private readonly Color[] consoleColors = new Color[Enum.GetValues<ConsoleColor>().Length];

    public Color DefaultBackground { get; set; }
    public Color DefaultForeground { get; set; }
    
    public void SetConsoleColor(ConsoleColor consoleColor, Color color)
    {
        consoleColors[(int)consoleColor] = color;
    }

    public Color GetConsoleColor(ConsoleColor consoleColor)
    {
        return consoleColors[(int)consoleColor];
    }

    public static readonly TerminalColorPalette Default = CreateDefaultInternal();

    private static TerminalColorPalette CreateDefaultInternal()
    {
        var palette = new TerminalColorPalette();

        palette.DefaultBackground = new Color(0x22, 0x22, 0x22);
        palette.DefaultForeground = Color.LightGray;
        
        palette.SetConsoleColor(ConsoleColor.Black,       Color.Black);
        palette.SetConsoleColor(ConsoleColor.DarkRed,     Color.DarkRed);
        palette.SetConsoleColor(ConsoleColor.DarkGreen,   Color.DarkGreen);
        palette.SetConsoleColor(ConsoleColor.DarkBlue,    Color.DarkBlue);
        palette.SetConsoleColor(ConsoleColor.DarkCyan,    Color.DarkCyan);
        palette.SetConsoleColor(ConsoleColor.DarkMagenta, Color.DarkMagenta);
        palette.SetConsoleColor(ConsoleColor.DarkYellow,  Color.Orange);
        palette.SetConsoleColor(ConsoleColor.DarkGray,    Color.DarkGray);
        palette.SetConsoleColor(ConsoleColor.Red,         Color.Red);
        palette.SetConsoleColor(ConsoleColor.Green,       Color.Green);
        palette.SetConsoleColor(ConsoleColor.Blue,        Color.Blue);
        palette.SetConsoleColor(ConsoleColor.Cyan,        Color.Cyan);
        palette.SetConsoleColor(ConsoleColor.Magenta,     Color.Magenta);
        palette.SetConsoleColor(ConsoleColor.Yellow,      Color.Yellow);
        palette.SetConsoleColor(ConsoleColor.Gray,        Color.Gray);
        palette.SetConsoleColor(ConsoleColor.White,       Color.White);
        
        return palette;
    }
}