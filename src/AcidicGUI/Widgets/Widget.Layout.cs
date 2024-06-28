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
                left = availableSpace.Left;
                width = availableSpace.Width;
                break;
            }
            case HorizontalAlignment.Left:
            {
                left = availableSpace.Left;
                width = contentSize.X;
                break;
            }
            case HorizontalAlignment.Center:
            {
                width = contentSize.X;
                left = availableSpace.Left + ((availableSpace.Width - width) / 2);
                break;
            }
            case HorizontalAlignment.Right:
            {
                width = contentSize.X;
                left = availableSpace.Right - width;
                break;
            }
        }

        switch (verticalAlignment)
        {
            case VerticalAlignment.Stretch:
            {
                top = availableSpace.Top;
                height = availableSpace.Height;
                break;
            }
            case VerticalAlignment.Top:
            {
                top = availableSpace.Top;
                height = contentSize.Y;
                break;
            }
            case VerticalAlignment.Middle:
            {
                height = contentSize.Y;
                top = availableSpace.Top + ((availableSpace.Height - height) / 2);
                break;
            }
            case VerticalAlignment.Bottom:
            {
                height = contentSize.Y;
                top = availableSpace.Bottom - height;
                break;
            }
        }

        calculatedLayoutRect = new LayoutRect(left, top, width, height);

        ArrangeChildren(context, calculatedLayoutRect);
        
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
    }
}