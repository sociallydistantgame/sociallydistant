using AcidicGUI.CustomProperties;
using AcidicGUI.Rendering;
using AcidicGUI.TextRendering;
using AcidicGUI.VisualStyles;

namespace AcidicGUI.Widgets;

public abstract partial class Widget : IFontProvider
{
    private readonly WidgetCollection children;
    private readonly Dictionary<Type, CustomPropertyObject> customProperties = new();

    private IVisualStyle? visualStyleOverride;
    private Widget? parent;
    private GuiManager? guiManager;
    private GuiMesh? cachedGeometry;

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
        
    }
    
    internal void RenderInternal(GuiRenderer renderer)
    {
        if (cachedGeometry == null)
        {
            var geometryHelper = new GeometryHelper(renderer);
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