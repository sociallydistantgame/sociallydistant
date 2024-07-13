using Microsoft.Xna.Framework;
using AcidicGUI.Layout;

namespace AcidicGUI.Widgets;

public sealed class StackPanel : ContainerWidget
{
    private int spacing;
    private Direction direction;

    public int Spacing
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

    protected override Point GetContentSize(Point availableSize)
    {
        Point result = Point.Zero;

        switch (direction)
        {
            case Direction.Horizontal:
            {
                result.X = spacing * Children.Count;
                availableSize.X = Math.Max(0, availableSize.X - result.X);
                break;
            }
            case Direction.Vertical:
            {
                result.Y = spacing * Children.Count;
                availableSize.Y = Math.Max(0, availableSize.Y - result.Y);
                break;
            }
        }
        
        foreach (Widget child in Children)
        {
            var childSize = child.GetCachedContentSize(availableSize);

            switch (direction)
            {
                case Direction.Horizontal:
                {
                    result.X += childSize.X;
                    result.Y = Math.Max(result.Y, childSize.Y);
                    availableSize.Y = Math.Max(0, availableSize.X - childSize.X);
                    break;
                }
                case Direction.Vertical:
                {
                    result.Y += childSize.Y;
                    result.X = Math.Max(result.X, childSize.X);
                    availableSize.Y = Math.Max(0, availableSize.Y - childSize.Y);
                    break;
                }
            }
        }

        return result;
    }

    protected override void ArrangeChildren(IGuiContext context, LayoutRect availableSpace)
    {
        int stride = 0;

        foreach (Widget child in Children)
        {
            Point childSize = child.GetCachedContentSize(availableSpace.Size);

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