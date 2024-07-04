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

    public static implicit operator Rectangle(LayoutRect rect)
        => new((int) rect.Left, (int) rect.Top, (int) rect.Width, (int) rect.Height);

    public bool Contains(Vector2 vector)
    {
        if (vector.X > Right)
            return false;

        if (vector.X < Left)
            return false;

        if (vector.Y < Top)
            return false;

        if (vector.Y > Bottom)
            return false;

        return true;
    }

    public static LayoutRect GetIntersection(LayoutRect a, LayoutRect b)
    {
        // thank you chatgpt, too fucking lazy to write this.
        float x1 = Math.Max(a.Left, b.Left);
        float y1 = Math.Max(a.Top, b.Top);
        float x2 = Math.Min(a.Right, b.Right);
        float y2 = Math.Min(a.Bottom, b.Bottom);

        if (x1 < x2 && y1 < y2)
        {
            return new LayoutRect(x1, y1, x2 - x1, y2 - y1);
        }
        else
        {
            // No intersection
            return new LayoutRect(0, 0, 0, 0);
        }
    }
}

public enum ClippingMode
{
    Inherit,
    Clip,
    DoNotClip
}