using AcidicGUI.Rendering;
using AcidicGUI.TextRendering;
using Microsoft.Xna.Framework;

namespace AcidicGUI.Widgets;

public sealed class Icon : Widget
{
    private Color color = Color.White;
    private string iconString = string.Empty;
    private int iconSize = 24;

    public string IconString
    {
        get => iconString;
        set
        {
            iconString = value;
            InvalidateLayout();
        }
    }

    public int IconSize
    {
        get => iconSize;
        set
        {
            iconSize = value;
            InvalidateLayout();
        }
    }

    public Color Color
    {
        get => color;
        set
        {
            color = value;
            InvalidateGeometry();
        }
    }

    protected override Vector2 GetContentSize(Vector2 availableSize)
    {
        Font? iconFont = GetVisualStyle().IconFont;
        if (iconFont == null)
            return Vector2.Zero;

        return new Vector2(iconSize, iconSize);
    }

    protected override void RebuildGeometry(GeometryHelper geometry)
    {
        var font = GetVisualStyle().IconFont;
        
        float x = ContentArea.Left + (ContentArea.Width - iconSize) / 2;
        float y = ContentArea.Top + (ContentArea.Height - iconSize) / 2;

        if (font == null)
            return;
        
        font.Draw(geometry, new Vector2(x, y), color, iconString, iconSize);
    }
}