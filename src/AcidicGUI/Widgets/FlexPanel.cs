using AcidicGUI.CustomProperties;
using AcidicGUI.Layout;
using Microsoft.Xna.Framework;

namespace AcidicGUI.Widgets;

public class FlexPanel : ContainerWidget
{
    private Direction direction;
    private float spacing;

    public IOrderedCollection<Widget> ChildWidgets => Children;

    public Direction Direction
    {
        get => direction;
        set
        {
            direction = value;
            InvalidateLayout();
        }
    }

    public float Spacing
    {
        get => spacing;
        set
        {
            spacing = value;
            InvalidateLayout();
        }
    }

    protected override Vector2 GetContentSize()
    {
        var result = Vector2.Zero;

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
        var resultSizes = new float?[Children.Count];
        var originalSizes = new float[Children.Count];
        var settingsObjects = new FlexPanelProperties[Children.Count];

        var availableSize = Direction == Direction.Horizontal
            ? availableSpace.Width - (spacing * originalSizes.Length)
            : availableSpace.Height - (spacing * originalSizes.Length);

        var i = 0;
        var proportionalCount = Children.Count;
        
        // Pass 1: Auto-sized elements
        foreach (Widget child in Children)
        {
            var childSize = child.GetCachedContentSize();
            var properties = child.GetCustomProperties<FlexPanelProperties>();

            settingsObjects[i] = properties;

            switch (Direction)
            {
                case Direction.Horizontal:
                {
                    originalSizes[i] = childSize.X;
                    break;
                }
                case Direction.Vertical:
                {
                    originalSizes[i] = childSize.Y;
                    break;
                }
            }
            
            if (properties.Mode == FlexMode.AutoSize)
            {
                resultSizes[i] = originalSizes[i];
                availableSize -= originalSizes[i];
                proportionalCount--;
            }

            i++;
        }

        i = 0;
        
        // Pass 2: Proportional elements and sending layout updates to children
        float stride = 0;
        foreach (Widget child in Children)
        {
            if (!resultSizes[i].HasValue)
            {
                float spaceAllowed = availableSize / proportionalCount;
                float spaceWanted = spaceAllowed * settingsObjects[i].Percentage;

                if (spaceWanted <= 0)
                {
                    spaceWanted = originalSizes[i];
                }

                availableSize -= spaceWanted;
                proportionalCount--;

                resultSizes[i] = spaceWanted;
            }

            float spaceGiven = resultSizes[i]!.Value;

            switch (direction)
            {
                case Direction.Horizontal:
                {
                    child.UpdateLayout(context, new LayoutRect(availableSpace.Left + stride, availableSpace.Top, spaceGiven, availableSpace.Height));
                    break;
                }
                case Direction.Vertical:
                {
                    child.UpdateLayout(context,
                        new LayoutRect(availableSpace.Left, availableSpace.Top + stride, availableSpace.Width,
                            spaceGiven));
                    break;
                }
            }
            
            stride += spaceGiven + spacing;
            i++;
        }
    }
}