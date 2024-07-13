using Microsoft.Xna.Framework;

namespace AcidicGUI.Layout;

public struct LayoutRect
{
    public readonly int Left;
    public readonly int Top;
    public readonly int Width;
    public readonly int Height;

    public int Right => Left + Width;
    public int Bottom => Top + Height;

    public Point TopLeft => new Point(Left, Top);
    public Point Size => new Point(Width,   Height);
    public Point Center => TopLeft + new Point(Width / 2, Height / 2);
    
    public LayoutRect(int left, int top, int width, int height)
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

    public bool Contains(Vector2 position) => Contains(position.ToPoint());
    
    public bool Contains(Point vector)
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
        int x1 = Math.Max(a.Left, b.Left);
        int y1 = Math.Max(a.Top, b.Top);
        int x2 = Math.Min(a.Right, b.Right);
        int y2 = Math.Min(a.Bottom, b.Bottom);

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

    public static bool operator ==(LayoutRect left, LayoutRect right)
    {
        return (Math.Abs(left.Left - right.Left) < float.Epsilon) && (Math.Abs(left.Top - right.Top) < float.Epsilon) && (Math.Abs(left.Width - right.Width) < float.Epsilon) && (Math.Abs(left.Height - right.Height) < float.Epsilon);
    }

    public static bool operator !=(LayoutRect left, LayoutRect right)
    {
        return !(left == right);
    }

    public override bool Equals(object? obj)
    {
        return obj is LayoutRect right && this == right;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Left, Top, Width, Height);
    }
}

public enum ClippingMode
{
    Inherit,
    Clip,
    DoNotClip
}