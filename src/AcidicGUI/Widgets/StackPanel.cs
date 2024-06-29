using Microsoft.Xna.Framework;
using AcidicGUI.Layout;

namespace AcidicGUI.Widgets;

public sealed class StackPanel : Widget
{
    private float spacing;
    private Direction direction;

    public float Spacing
    {
        get => spacing;
        set
        {
            spacing = value;
            InvalidateLayout();
        }
    }

    public Direction Direction
    {
        get => direction;
        set
        {
            direction = value;
            InvalidateLayout();
        }
    }
    
    public IOrderedCollection<Widget> ChildWidgets => Children;

    protected override Vector2 GetContentSize()
    {
        Vector2 result = Vector2.Zero;

        switch (direction)
        {
            case Direction.Horizontal:
            {
                result.X = spacing * Children.Count;
                break;
            }
            case Direction.Vertical:
            {
                result.Y = spacing * Children.Count;
                break;
            }
        }
        
        foreach (Widget child in Children)
        {
            var childSize = child.GetCachedContentSize();

            switch (direction)
            {
                case Direction.Horizontal:
                {
                    result.X += childSize.X;
                    result.Y = MathF.Max(result.Y, childSize.Y);
                    break;
                }
                case Direction.Vertical:
                {
                    result.Y += childSize.Y;
                    result.X = MathF.Max(result.X, childSize.X);
                    break;
                }
            }
        }

        return result;
    }

    protected override void ArrangeChildren(IGuiContext context, LayoutRect availableSpace)
    {
        float stride = 0;

        foreach (Widget child in Children)
        {
            Vector2 childSize = child.GetCachedContentSize();

            switch (direction)
            {
                case Direction.Horizontal:
                {
                    LayoutRect childSpace = new LayoutRect(availableSpace.Left + stride, availableSpace.Top,
                        childSize.X, availableSpace.Height);

                    stride += childSize.X + spacing;
                    child.UpdateLayout(context, childSpace);
                    break;
                }
                case Direction.Vertical:
                {
                    LayoutRect childSpace = new LayoutRect(availableSpace.Left, availableSpace.Top + stride,
                        availableSpace.Width, childSize.Y);

                    stride += childSize.Y + spacing;
                    child.UpdateLayout(context, childSpace);
                    break;
                }
            }
        }
    }
}