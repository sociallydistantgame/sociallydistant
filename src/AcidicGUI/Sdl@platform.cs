using System.Text;
using Silk.NET.SDL;

namespace AcidicGUI;

internal static class Sdl_platform
{
    private static readonly Sdl  sdl = Sdl.GetApi();
    private static          bool isInitialized;

    public static string GetClipboardText()
    {
        InitializeIfNeeded();
        return sdl.GetClipboardTextS();
    }

    public static void SetClipboardText(string text)
    {
        InitializeIfNeeded();
        sdl.SetClipboardText(Encoding.UTF8.GetBytes(text));
    }
    
    private static void InitializeIfNeeded()
    {
        if (isInitialized)
            return;

        sdl.Init(Sdl.InitVideo);
        
        AppDomain.CurrentDomain.ProcessExit += OnExit;
        
        isInitialized = true;
    }

    private static void OnExit(object? sender, EventArgs e)
    {
        if (!isInitialized)
            return;

        sdl.Dispose();
    }
}