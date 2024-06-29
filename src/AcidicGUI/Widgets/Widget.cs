using AcidicGUI.CustomProperties;
using AcidicGUI.Rendering;

namespace AcidicGUI.Widgets;

public abstract partial class Widget
{
    private readonly WidgetCollection children;
    private readonly Dictionary<Type, CustomPropertyObject> customProperties = new();
    
    private Widget? parent;
    private GuiManager? guiManager;
    private GuiMesh? cachedGeometry;

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

    protected virtual void RebuildGeometry(GeometryHelper geometry)
    {
        
    }
    
    internal void RenderInternal(GuiBatcher batcher)
    {
        if (cachedGeometry == null)
        {
            var geometryHelper = new GeometryHelper();
            RebuildGeometry(geometryHelper);
            cachedGeometry = geometryHelper.ExportMesh();
        }
        
        batcher.BatchGuiMesh(cachedGeometry.Value);

        foreach (Widget child in children)
        {
            child.RenderInternal(batcher);
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
}