using AcidicGUI.Rendering;
using AcidicGUI.TextRendering;
using Microsoft.Xna.Framework;

namespace AcidicGUI.Widgets;

public class TextWidget : Widget
{
    private FontInfo font;
    private string text = string.Empty;

    public string Text
    {
        get => text;
        set
        {
            text = value;
            InvalidateLayout();
        }
    }
    
    public FontInfo Font
    {
        get => font;
        set
        {
            font = value;
            InvalidateLayout();
        }
    }

    protected override Vector2 GetContentSize()
    {
        return font.GetFont(this).Measure(text);
    }

    protected override void RebuildGeometry(GeometryHelper geometry)
    {
        var fontInstance = font.GetFont(this);
        
        fontInstance.Draw(geometry, ContentArea.TopLeft, Color.White, text);
    }
}