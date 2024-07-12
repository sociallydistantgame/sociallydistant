namespace AcidicGUI.Layout;

public struct Padding
{
    public int Left;
    public int Top;
    public int Right;
    public int Bottom;

    public int Vertical => Top + Bottom;
    public int Horizontal => Left + Right;

    public Padding(int left, int top, int right, int bottom)
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }

    public Padding(int horizontal, int vertical) : this(horizontal, vertical, horizontal, vertical)
    { }
    
    public Padding(int uniform) : this(uniform, uniform)
    { }
    
    public static implicit operator Padding(int uniform)
        => new Padding(uniform);

    public static bool operator ==(Padding left, Padding right)
    {
        return left.Left == right.Left && left.Top == right.Top && left.Right == right.Right && left.Bottom == right.Bottom;
    }

    public static bool operator !=(Padding left, Padding right)
    {
        return !(left == right);
    }

    public override bool Equals(object? obj)
    {
        return obj is Padding right && this == right;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Left, Top, Bottom, Right);
    }
}