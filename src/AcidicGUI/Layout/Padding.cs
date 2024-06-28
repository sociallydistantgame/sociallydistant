namespace AcidicGUI.Layout;

public struct Padding
{
    public float Left;
    public float Top;
    public float Right;
    public float Bottom;

    public float Vertical => Top + Bottom;
    public float Horizontal => Left + Right;

    public Padding(float left, float top, float right, float bottom)
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }

    public Padding(float horizontal, float vertical) : this(horizontal, vertical, horizontal, vertical)
    { }
    
    public Padding(float uniform) : this(uniform, uniform)
    { }
    
    public static implicit operator Padding(float uniform)
        => new Padding(uniform);
}