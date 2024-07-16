using AcidicGUI.CustomProperties;
using AcidicGUI.Layout;
using Microsoft.Xna.Framework;

namespace AcidicGUI.Widgets;

public class FlexPanel : ContainerWidget
{
    private Direction direction;
    private int       spacing;
    private bool      reversed;

    public IOrderedCollection<Widget> ChildWidgets => Children;

    public bool Reversed
    {
        get => reversed;
        set
        {
            reversed = value;
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

    public int Spacing
    {
        get => spacing;
        set
        {
            spacing = value;
            InvalidateLayout();
        }
    }

    protected override Point GetContentSize(Point availableSize)
    {
        var result = Point.Zero;

        var proportionals = new Widget[Children.Count];
        var proportionalCount = 0;

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

        // Pass 1: Measure non-proportionals and figure out how much space we have for proprtionals
        foreach (Widget child in Children)
        {
            var settings = child.GetCustomProperties<FlexPanelProperties>();

            if (settings.Mode == FlexMode.Proportional)
            {
                proportionals[proportionalCount] = child;
                proportionalCount++;
                continue;
            }
            
            var childSize = child.GetCachedContentSize(availableSize);

            switch (direction)
            {
                case Direction.Horizontal:
                {
                    result.X += childSize.X;
                    availableSize.X = Math.Max(0, availableSize.X - childSize.X);
                    result.Y = Math.Max(result.Y, childSize.Y);
                    break;
                }
                case Direction.Vertical:
                {
                    result.Y += childSize.Y;
                    availableSize.Y = Math.Max(0, availableSize.Y - childSize.Y);
                    result.X = Math.Max(result.X, childSize.X);
                    break;
                }
            }
        }
        
        // Pass 2: Measure proportionals.
        for (var i = 0; i < proportionalCount; i++)
        {
            Widget child = proportionals[i];
            var settings = child.GetCustomProperties<FlexPanelProperties>();

            if (direction == Direction.Horizontal)
            {
                int space = (int) Math.Max(0, (availableSize.X / (float) proportionalCount) * settings.Percentage);
                var childSize = child.GetCachedContentSize(new Point(space, Math.Max(availableSize.Y, result.Y)));
                
                result.X += childSize.X;
                availableSize.X = Math.Max(0, availableSize.X - childSize.X);
                result.Y = Math.Max(result.Y, childSize.Y);
            }
            else if (direction == Direction.Vertical)
            {
                int space = (int) Math.Max(0, (availableSize.Y / (float) proportionalCount) * settings.Percentage);
                var childSize = child.GetCachedContentSize(new Point(Math.Max(availableSize.X, result.X), space));
                
                result.Y += childSize.Y;
                availableSize.Y = Math.Max(0, availableSize.Y - childSize.Y);
                result.X = Math.Max(result.X, childSize.X);
            }
        }
        
        return result;
    }

    protected override void ArrangeChildren(IGuiContext context, LayoutRect availableSpace)
    {
        var resultSizes = new int?[Children.Count];
        var originalSizes = new int[Children.Count];
        var settingsObjects = new FlexPanelProperties[Children.Count];

        var availableSizeForMeasure = availableSpace.Size;
        
        var availableSize = Direction == Direction.Horizontal
            ? availableSpace.Width - (spacing * originalSizes.Length)
            : availableSpace.Height - (spacing * originalSizes.Length);

        if (direction == Direction.Vertical)
        {
            availableSizeForMeasure.Y = availableSize;
        }
        else
        {
            availableSizeForMeasure.X = availableSize;
        }
        
        var i = 0;
        var proportionalCount = Children.Count;
        
        // Pass 1: Auto-sized elements
        foreach (Widget child in Children)
        {
            availableSizeForMeasure.X = Math.Max(0, availableSizeForMeasure.X);
            availableSizeForMeasure.Y = Math.Max(0, availableSizeForMeasure.Y);
            
            
            var childSize = child.GetCachedContentSize(availableSizeForMeasure);
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
                
                if (direction == Direction.Vertical)
                {
                    availableSizeForMeasure.Y = availableSize;
                }
                else
                {
                    availableSizeForMeasure.X = availableSize;
                }
                
                proportionalCount--;
            }

            i++;
        }

        i = 0;
        
        // Pass 2: Proportional elements and sending layout updates to children
        int stride = 0;
        foreach (Widget child in Children)
        {
            if (!resultSizes[i].HasValue)
            {
                int spaceAllowed = availableSize / proportionalCount;
                int spaceWanted = (int)(spaceAllowed * settingsObjects[i].Percentage);

                if (spaceWanted <= 0)
                {
                    spaceWanted = originalSizes[i];
                }

                availableSize -= spaceWanted;
                proportionalCount--;

                resultSizes[i] = spaceWanted;
            }

            int spaceGiven = resultSizes[i]!.Value;

            if (reversed)
            {
                stride -= spaceGiven;

                switch (direction)
                {
                    case Direction.Horizontal:
                    {
                        child.UpdateLayout(context, new LayoutRect(availableSpace.Right + stride, availableSpace.Top, spaceGiven, availableSpace.Height));
                        break;
                    }
                    case Direction.Vertical:
                    {
                        child.UpdateLayout(context, new LayoutRect(availableSpace.Left, availableSpace.Bottom + stride, availableSpace.Width, spaceGiven));
                        break;
                    }
                }
                
                stride -= spacing;
            }
            else
            {
                switch (direction)
                {
                    case Direction.Horizontal:
                    {
                        child.UpdateLayout(context, new LayoutRect(availableSpace.Left + stride, availableSpace.Top, spaceGiven, availableSpace.Height));
                        break;
                    }
                    case Direction.Vertical:
                    {
                        child.UpdateLayout(context, new LayoutRect(availableSpace.Left, availableSpace.Top + stride, availableSpace.Width, spaceGiven));
                        break;
                    }
                }
                
                stride += spaceGiven + spacing;
            }

            i++;
        }
    }
}