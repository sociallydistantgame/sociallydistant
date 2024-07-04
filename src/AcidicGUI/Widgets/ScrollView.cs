using AcidicGUI.Layout;
using AcidicGUI.Rendering;
using Microsoft.Xna.Framework;

namespace AcidicGUI.Widgets;

public sealed class ScrollView : ContainerWidget
{
    private float spacing;
    private float innerSize;
    private float pageOffset;
    private bool showScrollBar;

    public float Spacing
    {
        get => spacing;
        set
        {
            spacing = value;
            InvalidateLayout();
        }
    }

    public ScrollView()
    {
        LayoutRoot = this;
        ClippingMode = ClippingMode.Clip;
    }
    
    protected override Vector2 GetContentSize(Vector2 availableSize)
    {
        innerSize = spacing * (Children.Count - 1);

        // Figure out (roughly) how tall the inner content is, so we know if we should display a scrollbar.
        // If we do, then we will need to arrange children with that in mind (so scrollbar doesn't overlap them)
        foreach (Widget child in Children)
        {
            var childAvailable = new Vector2(availableSize.X, 0);
            var childSize = child.GetCachedContentSize(childAvailable);

            innerSize += childSize.Y;
        }

        showScrollBar = innerSize > availableSize.Y;

        // The size we just calculated doesn't matter for our own measurement, we're a layout root and must take all the space the parent gives us.
        return availableSize;
    }

    protected override void ArrangeChildren(IGuiContext context, LayoutRect availableSpace)
    {
        var offset = 0f;

        if (showScrollBar)
        {
            availableSpace = new LayoutRect(
                availableSpace.Left,
                availableSpace.Top,
                availableSpace.Width - GetVisualStyle().ScrollBarSize,
                availableSpace.Height
            );
        }
        
        // Pass 1: Determine max page offset
        innerSize = spacing * Children.Count;
        foreach (Widget child in Children)
        {
            var childSize = child.GetCachedContentSize(availableSpace.Size);
            innerSize += childSize.Y;
        }

        // Make sure we adjust the scroll page offset in case we've lost some inner height.
        pageOffset = Math.Clamp(pageOffset, 0, innerSize - availableSpace.Height);
        
        // Pass 2: Layout updates
        foreach (Widget child in Children)
        {
            var childSize = child.GetCachedContentSize(availableSpace.Size);

            child.UpdateLayout(context, new LayoutRect(
                availableSpace.Left,
                availableSpace.Top - pageOffset + offset,
                availableSpace.Width,
                childSize.Y
            ));

            offset += childSize.Y + spacing;
        }
    }

    protected override void RebuildGeometry(GeometryHelper geometry)
    {
        base.RebuildGeometry(geometry);

        if (showScrollBar)
        {
            GetVisualStyle().DrawScrollBar(this, geometry, new LayoutRect(
                ContentArea.Right - GetVisualStyle().ScrollBarSize,
                ContentArea.Top,
                GetVisualStyle().ScrollBarSize,
                ContentArea.Height
            ), pageOffset, innerSize);
        }
    }
}