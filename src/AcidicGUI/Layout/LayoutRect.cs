namespace AcidicGUI.Layout;

public struct LayoutRect
{
    public readonly float Left;
    public readonly float Top;
    public readonly float Width;
    public readonly float Height;

    public float Right => Left + Width;
    public float Bottom => Top + Height;

    public LayoutRect(float left, float top, float width, float height)
    {
        Left = left;
        Top = top;
        Width = width;
        Height = height;
    }
}