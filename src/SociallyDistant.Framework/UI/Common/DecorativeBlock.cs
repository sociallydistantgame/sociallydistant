using Microsoft.Xna.Framework;
using AcidicGUI.Widgets;

namespace SociallyDistant.Core.UI.Common;

public sealed class DecorativeBlock : Widget
{
    private Color? color;
    private bool opaque;

    public bool Opaque
    {
        get => opaque;
        set
        {
            opaque = value;
            InvalidateGeometry();
        }
    }

    public Color? BoxColor
    {
        get => color;
        set
        {
            color = value;
            InvalidateGeometry();
        }
    }
}