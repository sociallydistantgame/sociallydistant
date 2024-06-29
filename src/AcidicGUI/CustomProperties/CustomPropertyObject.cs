using AcidicGUI.Layout;
using AcidicGUI.Widgets;
using Microsoft.Xna.Framework;

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

public sealed class FlexPanelProperties : CustomPropertyObject
{
    private float proportionalValue = 1;
    private FlexMode mode;

    public float Percentage
    {
        get => proportionalValue;
        set
        {
            proportionalValue = MathHelper.Clamp(value, 0f, 1f);
            Widget.InvalidateLayout();
        }
    }
    
    public FlexMode Mode
    {
        get => mode;
        set
        {
            mode = value;
            Widget.InvalidateLayout();
        }
    }
    
    public FlexPanelProperties(Widget owner) : base(owner)
    {
    }
}