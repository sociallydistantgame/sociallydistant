using AcidicGUI.CustomProperties;
using AcidicGUI.Layout;
using AcidicGUI.Rendering;
using AcidicGUI.TextRendering;
using AcidicGUI.VisualStyles;
using Microsoft.Xna.Framework;

namespace AcidicGUI.Widgets;

public abstract partial class Widget : IFontFamilyProvider
{
    private readonly WidgetCollection children;
    private readonly Dictionary<Type, CustomPropertyObject> customProperties = new();

    private IVisualStyle? visualStyleOverride;
    private Widget?       parent;
    private GuiManager?   guiManager;
    private GuiMesh?      cachedGeometry;
    private float         renderOpacity = 1;
    private bool          enabled       = true;
    private ClippingMode  clippingMode;
    private LayoutRect    clipRect;

    public bool IsFocused
    {
        get
        {
            if (GuiManager == null)
                return false;

            return GuiManager.IsFocused(this);
        }
    }

    public bool IsChildFocused => IsFocused || children.Any(x => x.IsChildFocused);

    public LayoutRect ClippedContentArea => clipRect;
    
    public ClippingMode ClippingMode
    {
        get => clippingMode;
        set
        {
            clippingMode = value;
            InvalidateGeometry(true);
        }
    }
    
    public bool Enabled
    {
        get => enabled;
        set
        {
            enabled = value;
            InvalidateGeometry(true);
        }
    }
    
    public float RenderOpacity
    {
        get => renderOpacity;
        set
        {
            renderOpacity = MathHelper.Clamp(value, 0, 1);
            this.InvalidateGeometry(true);
        }
    }

    public bool HierarchyEnabled
    {
        get
        {
            // TODO: Caching caching caching!
            if (Parent == null)
                return enabled;

            return Parent.HierarchyEnabled && enabled;
        }
    }
    
    public float ComputedOpacity
    {
        get
        {
            // TODO: Caching caching caching!
            if (Parent == null)
                return renderOpacity;

            return Parent.ComputedOpacity * renderOpacity;
        }
    }
    
    public IVisualStyle? VisualStyleOverride
    {
        get => visualStyleOverride;
        set
        {
            visualStyleOverride = value;
            InvalidateLayout();
        }
    }
    
    public Widget? Parent
    {
        get => parent;
        private set => parent = value;
    }

    public Widget? Root
    {
        get
        {
            if (parent == null)
                return this;

            return parent.Root;
        }
    }
    
    public GuiManager? GuiManager
    {
        get
        {
            if (parent != null)
                return parent.GuiManager;

            return guiManager;
        }
    }

    protected IOrderedCollection<Widget> Children => children;

    public Widget()
    {
        this.children = new WidgetCollection(this);
    }

    public void InvalidateGeometry(bool invalidateChildren = false)
    {
        cachedGeometry = null;

        if (invalidateChildren)
        {
            foreach (Widget child in children)
                child.InvalidateGeometry(invalidateChildren);
        }
    }
    
    public IVisualStyle GetVisualStyle()
    {
        if (visualStyleOverride != null)
            return visualStyleOverride;

        if (parent != null)
            return parent.GetVisualStyle();

        return GuiManager.GetVisualStyle();
    }
    
    protected virtual void RebuildGeometry(GeometryHelper geometry)
    {
        GetVisualStyle().DrawWidgetBackground(this, geometry);
    }

    private LayoutRect? GetClippingRectangle()
    {
        if (clippingMode == ClippingMode.Inherit || clippingMode == ClippingMode.DoNotClip)
        {
            // Toplevels do not clip if clipping mode is set to inherit.
            if (parent == null)
                return null;

            return parent.GetClippingRectangle();
        }
        
        // get the parent clipping rectangle so we can clamp ours within.
        LayoutRect? parentRect = parent?.GetClippingRectangle();

        LayoutRect ourRect = ContentArea;

        if (parentRect != null)
        {
            ourRect = LayoutRect.GetIntersection(ourRect, parentRect.Value);
        }

        return ourRect;
    }
    
    internal void RenderInternal(GuiRenderer renderer)
    {
        if (visibility != Visibility.Visible)
            return;
        
        if (cachedGeometry == null)
        {
            var geometryHelper = new GeometryHelper(renderer, ComputedOpacity, !HierarchyEnabled, clipRect);
            RebuildGeometry(geometryHelper);
            cachedGeometry = geometryHelper.ExportMesh();
        }
        
        renderer.RenderGuiMesh(cachedGeometry.Value);

        foreach (Widget child in children)
        {
            child.RenderInternal(renderer);
        }
    }

    public T GetCustomProperty<T>() where T : struct
    {
        return GetCustomProperties<StructProperty<T>>().Value;
    }

    public void SetCustomProperty<T>(T newValue) where T : struct
    {
        GetCustomProperties<StructProperty<T>>().Value = newValue;
    }
    
    public T GetCustomProperties<T>() where T : CustomPropertyObject
    {
        var type = typeof(T);

        if (!customProperties.TryGetValue(type, out CustomPropertyObject? obj))
        {
            obj = (CustomPropertyObject)Activator.CreateInstance(type, new object[] { this })!;
            customProperties.Add(type, obj);
        }

        return (T)obj;
    }

    public IFontFamily GetFont(PresetFontFamily family)
    {
        return GuiManager.GetFont(family);
    }

    public void GiveFocus()
    {
        if (GuiManager == null)
            return;
        
        GuiManager.SetFocusedWidget(this);
    }
}