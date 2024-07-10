using AcidicGUI.Rendering;
using FontStashSharp;
using Microsoft.Xna.Framework;

namespace AcidicGUI.TextRendering;

internal sealed class DynamicFont : Font
{
    private readonly FontSystem fontSystem = new();
    private readonly float fontSize;

    
    
    static DynamicFont()
    {
        FontSystemDefaults.KernelWidth = 2;
        FontSystemDefaults.KernelHeight = 2;
        FontSystemDefaults.FontResolutionFactor = 2;
    }
    
    internal DynamicFont(float fontSize)
    {
        this.fontSize = fontSize;
    }
    
    internal void AddTtfStreamInternal(Stream stream)
    {
        this.fontSystem.AddFont(stream);
    }

    public override Vector2 Measure(string text, int? fontSizePixels)
    {
        if (string.IsNullOrEmpty(text))
        {
            return new Vector2(0,
                fontSystem.GetFont((fontSizePixels ?? fontSize) * 1.333f).LineHeight);
        }
        
        return fontSystem.GetFont((fontSizePixels ?? this.fontSize) * 1.333f).MeasureString(text);
    }

    public override void Draw(GeometryHelper geometryHelper, Vector2 position, Color color, string text, int? fontSizePixels)
    {
        fontSystem.GetFont((fontSizePixels ?? this.fontSize) * 1.333f).DrawText(geometryHelper, text, position, color);
    }

    public override float GetLineHeight(int? fontSizePixels)
    {
        return fontSystem.GetFont((fontSizePixels ?? fontSize) * 1.333f).LineHeight;
    }
}