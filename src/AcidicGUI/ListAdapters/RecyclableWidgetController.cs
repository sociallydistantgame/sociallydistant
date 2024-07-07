using AcidicGUI.Widgets;

namespace AcidicGUI.ListAdapters;

public abstract class RecyclableWidgetController
{
    public abstract void Build(ContentWidget destination);
    public abstract void Recycle();

    protected T GetWidget<T>()
        where T : Widget, new()
    {
        return RecycleBin.Get<T>().GetWidget();
    }
    
    protected void Recyclewidget<T>(T? widget)
        where T : Widget, new()
    {
        if (widget == null)
            return;
        
        RecycleBin.Get<T>().Recycle(widget);
    }
    
    
}