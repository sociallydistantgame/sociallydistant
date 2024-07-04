using AcidicGUI.CustomProperties;
using AcidicGUI.Rendering;
using AcidicGUI.TextRendering;
using AcidicGUI.VisualStyles;
using Microsoft.Xna.Framework;

namespace AcidicGUI.Widgets;

public abstract partial class Widget : IFontProvider
{
    private readonly WidgetCollection children;
    private readonly Dictionary<Type, CustomPropertyObject> customProperties = new();

    private IVisualStyle? visualStyleOverride;
    private Widget? parent;
    private GuiManager? guiManager;
    private GuiMesh? cachedGeometry;
    private float renderOpacity = 1;
    private bool enabled = true;

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
    
    internal void RenderInternal(GuiRenderer renderer)
    {
        if (cachedGeometry == null)
        {
            var geometryHelper = new GeometryHelper(renderer, ComputedOpacity, !HierarchyEnabled);
            RebuildGeometry(geometryHelper);
            cachedGeometry = geometryHelper.ExportMesh();
        }
        
        renderer.RenderGuiMesh(cachedGeometry.Value);

        foreach (Widget child in children)
        {
            child.RenderInternal(renderer);
        }
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

    public Font GetFont(FontPreset presetFont)
    {
        return GuiManager.GetFont(presetFont);
    }
}