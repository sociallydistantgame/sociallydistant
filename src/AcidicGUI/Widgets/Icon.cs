using AcidicGUI.Rendering;
using AcidicGUI.TextRendering;
using Microsoft.Xna.Framework;

namespace AcidicGUI.Widgets;

public sealed class Icon : Widget
{
    private Point  actualIconSize;
    private Color? color;
    private string iconString = string.Empty;
    private int    iconSize   = 24;

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

    public Color? Color
    {
        get => color;
        set
        {
            color = value;
            InvalidateGeometry();
        }
    }

    protected override Point GetContentSize(Point availableSize)
    {
        Font? iconFont = GetVisualStyle().IconFont;
        if (iconFont == null || string.IsNullOrEmpty(iconString))
            return Point.Zero;

        return actualIconSize = iconFont.Measure(iconString, iconSize);
    }

    protected override void RebuildGeometry(GeometryHelper geometry)
    {
        var font = GetVisualStyle().IconFont;
        
        float x = ContentArea.Left + (ContentArea.Width - actualIconSize.X) / 2;
        float y = ContentArea.Top + (ContentArea.Height - actualIconSize.Y) / 2;

        if (font == null)
            return;
        
        font.Draw(geometry, new Vector2(x, y), color ?? Microsoft.Xna.Framework.Color.White, iconString, iconSize);
    }
}