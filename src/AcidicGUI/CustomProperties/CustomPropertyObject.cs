using AcidicGUI.Widgets;

namespace AcidicGUI.CustomProperties;

public abstract class CustomPropertyObject
{
    private readonly Widget widget;

    protected Widget Widget => widget;

    public CustomPropertyObject(Widget owner)
    {
        this.widget = owner;
    }
}