using AcidicGUI.Widgets;

namespace AcidicGUI.CustomProperties;

public sealed class StructProperty<T> : CustomPropertyObject
    where T : struct
{
    private T value;
    
    public T Value
    {
        get => this.value;
        set
        {
            this.value = value;
            this.Widget.InvalidateLayout();
        }
    }
    
    public StructProperty(Widget owner) : base(owner)
    {
    }
}