using AcidicGUI.Layout;
using Microsoft.Xna.Framework;

namespace AcidicGUI.Widgets;

public partial class Widget
{
    private Widget?             layoutRoot;
    private bool                layoutIsDirty = true;
    private HorizontalAlignment horizontalAlignment;
    private VerticalAlignment   verticalAlignment;
    private LayoutRect          geometryRect;
    private LayoutRect          calculatedLayoutRect;
    private Vector2?            cachedContentSize;
    private Padding             padding;
    private Padding             margin;
    private Vector2             minimumSize;
    private Vector2             maximumSize;
    private Vector2             previousAvailableSize;
    private Visibility          visibility;

    public Visibility Visibility
    {
        get => visibility;
        set
        {
            if (this.visibility == value)
                return;
            
            visibility = value;
            InvalidateLayout();
        }
    }

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

    public Vector2 MinimumSize
    {
        get => minimumSize;
        set
        {
            minimumSize = value;
            InvalidateLayout();
        }
    }
    
    public Vector2 MaximumSize
    {
        get => maximumSize;
        set
        {
            maximumSize = value;
            InvalidateLayout();
        }
    }

    public HorizontalAlignment HorizontalAlignment
    {
        get => horizontalAlignment;
        set
        {
            horizontalAlignment = value;
            InvalidateLayout();
        }
    }
    
    public VerticalAlignment VerticalAlignment
    {
        get => verticalAlignment;
        set
        {
            verticalAlignment = value;
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
        protected set
        {
            layoutRoot = value;
        }
    }

    public void InvalidateOwnLayout()
    {
        InvalidateLayoutInternal();
    }
    
    public void InvalidateLayout()
    {
        LayoutRoot.InvalidateLayoutInternal();
        
        GuiManager?.SubmitForLayoutUpdateInternal(LayoutRoot);
    }

    public void UpdateLayout(IGuiContext context, LayoutRect availableSpace)
    {
        if (!layoutIsDirty)
            return;

        if (visibility == Visibility.Collapsed)
        {
            this.calculatedLayoutRect = new LayoutRect(0, 0, 0, 0);
            layoutIsDirty = false;
            return;
        }
        
        var contentSize = GetCachedContentSize(availableSpace.Size);
        
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
                left = availableSpace.Left + (availableSpace.Width - width) / 2;
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
                top = availableSpace.Top + (availableSpace.Height - height) / 2;
                break;
            }
            case VerticalAlignment.Bottom:
            {
                height = contentSize.Y;
                top = availableSpace.Bottom - height;
                break;
            }
        }

        left += padding.Left;
        top += padding.Top;
        width -= padding.Horizontal;
        height -= padding.Vertical;
        
        calculatedLayoutRect = new LayoutRect(left, top, width, height);

        left += margin.Left;
        top += margin.Top;
        width -= margin.Horizontal;
        height -= margin.Vertical;

        ArrangeChildren(context, new LayoutRect(left, top, width, height));

        if (geometryRect != calculatedLayoutRect)
        {
            InvalidateGeometry();
            geometryRect = calculatedLayoutRect;
        }
        
        CalculateClipRect();
        
        layoutIsDirty = false;
    }

    private void CalculateClipRect()
    {
        LayoutRect? newClipRect = GetClippingRectangle();
        if (newClipRect == null)
            newClipRect = ContentArea;

        clipRect = LayoutRect.GetIntersection(newClipRect.Value, ContentArea);
    }
    
    public Vector2 GetCachedContentSize(Vector2 availableSize)
    {
        if (MaximumSize.X > 0 && availableSize.X > MaximumSize.X)
            availableSize.X = MaximumSize.X;
        if (MaximumSize.Y > 0 && availableSize.Y > MaximumSize.Y)
            availableSize.Y = MaximumSize.Y;

        if (previousAvailableSize != availableSize)
            cachedContentSize = null;
        
        if (cachedContentSize != null)
            return cachedContentSize.Value;

        if (this.visibility == Visibility.Collapsed)
        {
            cachedContentSize = Vector2.Zero;
            return cachedContentSize.Value;
        }
        
        Vector2 contentSize = GetContentSize(availableSize);
        
        contentSize.X += margin.Horizontal;
        contentSize.Y += margin.Vertical;
        contentSize.X += padding.Horizontal;
        contentSize.Y += padding.Vertical;
        
        
        if (maximumSize.X > 0)
        {
            contentSize.X = Math.Min(Math.Max(contentSize.X, minimumSize.X), maximumSize.X);
        }
        else
        {
            contentSize.X = Math.Max(contentSize.X, minimumSize.X);
        }

        if (maximumSize.Y > 0)
        {
            contentSize.Y = Math.Min(Math.Max(contentSize.Y, minimumSize.Y), maximumSize.Y);
        }
        else
        {
            contentSize.Y = Math.Max(contentSize.Y, minimumSize.Y);
        }

        cachedContentSize = contentSize;
        return cachedContentSize.Value;
    }
    
    protected virtual void ArrangeChildren(IGuiContext context, LayoutRect availableSpace)
    {
        foreach (Widget child in children)
            child.UpdateLayout(context, availableSpace);
    }
    
    protected virtual Vector2 GetContentSize(Vector2 availableSize)
    {
        var result = Vector2.Zero;
        
        foreach (Widget child in children)
        {
            Vector2 childSize = child.GetCachedContentSize(availableSize);

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