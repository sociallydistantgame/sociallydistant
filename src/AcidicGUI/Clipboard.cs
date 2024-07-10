namespace AcidicGUI;

public static class Clipboard
{
    public static string GetText()
    {
        return Sdl_platform.GetClipboardText();
    }
    
    public static void SetText(string text)
    {
        Sdl_platform.SetClipboardText(text);
    }
}