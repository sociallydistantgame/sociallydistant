using AcidicGUI.Rendering;
using FontStashSharp;
using Microsoft.Xna.Framework;

namespace AcidicGUI.TextRendering;

internal sealed class DynamicFont : Font
{
    private readonly FontSystem fontSystem = new();
    private readonly int fontSize;

    internal DynamicFont(int fontSize)
    {
        this.fontSize = fontSize;
    }
    
    internal void AddTtfStreamInternal(Stream stream)
    {
        this.fontSystem.AddFont(stream);
    }

    public override Vector2 Measure(string text)
    {
        return fontSystem.GetFont(fontSize).MeasureString(text);
    }

    public override void Draw(GeometryHelper geometryHelper, Vector2 position, Color color, string text)
    {
        fontSystem.GetFont(fontSize).DrawText(geometryHelper, text, position, color);
    }
}