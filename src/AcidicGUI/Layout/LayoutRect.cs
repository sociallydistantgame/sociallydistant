using Microsoft.Xna.Framework;

namespace AcidicGUI.Layout;

public struct LayoutRect
{
    public readonly float Left;
    public readonly float Top;
    public readonly float Width;
    public readonly float Height;

    public float Right => Left + Width;
    public float Bottom => Top + Height;

    public Vector2 TopLeft => new Vector2(Left, Top);
    public Vector2 Size => new Vector2(Width, Height);
    
    public LayoutRect(float left, float top, float width, float height)
    {
        Left = left;
        Top = top;
        Width = width;
        Height = height;
    }

    public static LayoutRect operator -(LayoutRect rect, Padding padding)
    {
        return new LayoutRect(rect.Left + padding.Left, rect.Top + padding.Top, rect.Width - padding.Horizontal,
            rect.Height - padding.Vertical);
    }
}