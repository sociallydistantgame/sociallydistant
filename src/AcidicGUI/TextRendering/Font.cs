using FontStashSharp;

namespace AcidicGUI.TextRendering;

public sealed class Font : IDisposable
{
    private readonly FontSystem fontSystem = new();

    private void AddTtfStreamInternal(Stream stream)
    {
        fontSystem.AddFont(stream);
    }
    
    public static Font FromTtfFile(string filePath)
    {
        using var fileStream = File.OpenRead(filePath);

        return FromTtfStream(fileStream);
    }

    public static Font FromTtfBytes(byte[] ttfBytes)
    {
        using var memory = new MemoryStream(ttfBytes);

        return FromTtfStream(memory);
    }

    public static Font FromTtfStream(Stream stream)
    {
        var font = new Font();

        font.AddTtfStreamInternal(stream);
        
        return font;
    }

    public void Dispose()
    {
        fontSystem.Dispose();
    }
}