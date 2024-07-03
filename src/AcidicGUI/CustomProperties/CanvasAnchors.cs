using Microsoft.Xna.Framework;
using AcidicGUI.Widgets;

namespace AcidicGUI.CustomProperties;

public sealed class CanvasAnchors : CustomPropertyObject
{
    private Vector2 anchoredPosition;
    private Vector2 pivot;

    public Vector2 AnchoredPosition
    {
        get => anchoredPosition;
        set
        {
            anchoredPosition = value;
            Widget.InvalidateLayout();
        }
    }

    public Vector2 Pivot
    {
        get => pivot;
        set
        {
            pivot = value;
            Widget.InvalidateLayout();
        }
    }
    
    public CanvasAnchors(Widget owner) : base(owner)
    {
    }
}