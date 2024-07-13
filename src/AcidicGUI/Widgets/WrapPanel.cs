using AcidicGUI.Layout;
using Microsoft.Xna.Framework;

namespace AcidicGUI.Widgets;

public sealed class WrapPanel : ContainerWidget
{
    private Direction direction;
    private int     spacingX;
    private int     spacingY;
    
    public Direction Direction
    {
        get => direction;
        set
        {
            direction = value;
            InvalidateLayout();
        }
    }
    
    public int SpacingX
    {
        get => spacingX;
        set
        {
            spacingX = value;
            InvalidateLayout();
        }
    }
    
    public int SpacingY
    {
        get => spacingY;
        set
        {
            spacingY = value;
            InvalidateLayout();
        }
    }

    protected override Point GetContentSize(Point availableSize)
    {
        Point result = Point.Zero;
        int lineWidth = 0;
        int lineHeight = 0;
        

        foreach (Widget child in Children)
        {
            Point childSize = child.GetCachedContentSize(availableSize);

            if (direction == Direction.Horizontal)
            {
                bool wrap = availableSize.X > 0 && lineWidth + childSize.X > availableSize.X;

                if (wrap)
                {
                    result.X = Math.Max(result.X, lineWidth);
                    result.Y += lineHeight + spacingY;
                    lineWidth = 0;
                    lineHeight = 0;
                }

                lineWidth += childSize.X + spacingX;
                lineHeight = Math.Max(lineHeight, childSize.Y);
            }
            else
            {
                bool wrap = availableSize.Y > 0 && lineHeight + childSize.Y > availableSize.Y;

                if (wrap)
                {
                    result.X += lineWidth + spacingX;
                    result.Y = Math.Max(result.Y, lineHeight);
                    lineWidth = 0;
                    lineHeight = 0;
                }

                lineHeight += childSize.Y + spacingY;
                lineWidth = Math.Max(lineWidth, childSize.X);
            }
        }

        if (direction == Direction.Horizontal)
        {
            result.X = Math.Max(result.X, lineWidth);
            result.Y += lineHeight;
        }
        else
        {
            result.Y = Math.Max(result.Y, lineHeight);
            result.X += lineWidth;
        }
        
        return result;
    }

    protected override void ArrangeChildren(IGuiContext context, LayoutRect availableSpace)
    {
        Point offset = Point.Zero;
        int lineSize = 0;

        foreach (Widget child in Children)
        {
            var childSize = child.GetCachedContentSize(availableSpace.Size);

            if (direction == Direction.Horizontal)
            {
                if (offset.X + childSize.X > availableSpace.Width)
                {
                    offset.X = 0;
                    offset.Y += lineSize + spacingY;
                    lineSize = 0;
                }

                child.UpdateLayout(context, new LayoutRect
                (
                    availableSpace.Left + offset.X,
                    availableSpace.Top + offset.Y,
                    childSize.X,
                    childSize.Y
                ));

                offset.X += childSize.X + spacingX;
                lineSize = Math.Max(lineSize, childSize.Y);
            }
            else
            {
                if (offset.Y + childSize.Y > availableSpace.Height)
                {
                    offset.Y = 0;
                    offset.X += lineSize + spacingX;
                    lineSize = 0;
                }

                child.UpdateLayout(context, new LayoutRect
                (
                    availableSpace.Left + offset.X,
                    availableSpace.Top + offset.Y,
                    childSize.X,
                    childSize.Y
                ));

                offset.Y += childSize.Y + spacingY;
                lineSize = Math.Max(lineSize, childSize.X);
            }
        }
    }
}