using System.Numerics;
using AcidicGUI.Layout;

namespace AcidicGUI.Widgets;

public partial class Widget
{
    private Widget? layoutRoot;
    private bool layoutIsDirty = true;
    private HorizontalAlignment horizontalAlignment;
    private VerticalAlignment verticalAlignment;
    private LayoutRect calculatedLayoutRect;
    private Vector2? cachedContentSize;
    private Padding padding;
    private Padding margin;

    public Padding Margin
    {
        get => margin;
        set
        {
            margin = value;
            InvalidateLayout();
        }
    }

    public Padding Padding
    {
        get => padding;
        set
        {
            padding = value;
            InvalidateLayout();
        }
    }
    
    public LayoutRect ContentArea => calculatedLayoutRect;
    
    public Widget LayoutRoot
    {
        get
        {
            if (layoutRoot == this)
                return layoutRoot;

            if (Parent == null)
                return this;

            return Parent.LayoutRoot;
        }
    }

    public void InvalidateLayout()
    {
        LayoutRoot.InvalidateLayoutInternal();
    }

    public void UpdateLayout(IGuiContext context, LayoutRect availableSpace)
    {
        if (!layoutIsDirty)
            return;

        var contentSize = GetCachedContentSize();
        
        // TODO: Padding and margin

        var left = 0f;
        var top = 0f;
        var width = 0f;
        var height = 0f;

        switch (horizontalAlignment)
        {
            case HorizontalAlignment.Stretch:
            {
                left = availableSpace.Left + padding.Left;
                width = availableSpace.Width - padding.Left - padding.Right;
                break;
            }
            case HorizontalAlignment.Left:
            {
                left = availableSpace.Left + padding.Left;
                width = contentSize.X;
                break;
            }
            case HorizontalAlignment.Center:
            {
                width = contentSize.X;
                left = availableSpace.Left + padding.Left + ((availableSpace.Width - (padding.Right + padding.Left) - width) / 2);
                break;
            }
            case HorizontalAlignment.Right:
            {
                width = contentSize.X;
                left = availableSpace.Right - padding.Right - width;
                break;
            }
        }

        switch (verticalAlignment)
        {
            case VerticalAlignment.Stretch:
            {
                top = availableSpace.Top + padding.Top;
                height = availableSpace.Height - padding.Top - padding.Bottom;
                break;
            }
            case VerticalAlignment.Top:
            {
                top = availableSpace.Top + padding.Top;
                height = contentSize.Y;
                break;
            }
            case VerticalAlignment.Middle:
            {
                height = contentSize.Y;
                top = availableSpace.Top + padding.Top + ((availableSpace.Height - padding.Vertical - height) / 2);
                break;
            }
            case VerticalAlignment.Bottom:
            {
                height = contentSize.Y;
                top = availableSpace.Bottom - padding.Bottom - height;
                break;
            }
        }

        calculatedLayoutRect = new LayoutRect(left, top, width, height);

        ArrangeChildren(context, calculatedLayoutRect - margin);
        
        layoutIsDirty = false;
    }

    public Vector2 GetCachedContentSize()
    {
        if (cachedContentSize != null)
            return cachedContentSize.Value;

        cachedContentSize = GetContentSize();
        return cachedContentSize.Value;
    }
    
    protected virtual void ArrangeChildren(IGuiContext context, LayoutRect availableSpace)
    {
        foreach (Widget child in children)
            child.UpdateLayout(context, availableSpace);
    }
    
    protected virtual Vector2 GetContentSize()
    {
        var result = Vector2.Zero;
        
        foreach (Widget child in children)
        {
            Vector2 childSize = child.GetContentSize();

            result.X = MathF.Max(result.X, childSize.X);
            result.Y = MathF.Max(result.Y, childSize.Y);
        }

        return result;
    }

    private void InvalidateLayoutInternal()
    {
        foreach (Widget child in children)
            child.InvalidateLayoutInternal();
        
        layoutIsDirty = true;
        cachedContentSize = null;
        cachedGeometry = null;
    }
}