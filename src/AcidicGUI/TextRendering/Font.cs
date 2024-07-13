using AcidicGUI.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AcidicGUI.TextRendering;

public abstract class Font : IDisposable
{
    private bool disposed = false;

    public abstract Point Measure(string text, int? fontSize = null);
    public abstract void Draw(GeometryHelper geometryHelper, Vector2 position, Color color, string text, int? fontSize = null);

    public abstract int GetLineHeight(int? fontSizePixels);
    
    public static Font FromTtfFile(string filePath, int fontSize)
    {
        using var fileStream = File.OpenRead(filePath);

        return FromTtfStream(fileStream, fontSize);
    }

    public static Font FromTtfBytes(byte[] ttfBytes, int fontSize)
    {
        using var memory = new MemoryStream(ttfBytes);

        return FromTtfStream(memory, fontSize);
    }

    public static Font FromTtfStream(Stream stream, int fontSize)
    {
        var font = new DynamicFont(fontSize);

        font.AddTtfStreamInternal(stream);
        
        return font;
    }

    public void Dispose()
    {
        Dispose(!disposed);
        disposed = true;
    }

    public static implicit operator Font(SpriteFont spriteFont)
    {
        return new SimpleFont(spriteFont);
    }
    
    protected virtual void Dispose(bool disposing) 
    { }
}