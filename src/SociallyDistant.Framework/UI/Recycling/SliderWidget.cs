using AcidicGUI.ListAdapters;

namespace SociallyDistant.Core.UI.Recycling;

public class SliderWidget : IWidget
{
    public float MinimumValue { get; set; }
    public float MaximumValue { get; set; }
    public float Value { get; set; }
    public Action<float>? Callback { get; set; }
    public RecyclableWidgetController Build()
    {
        return new SliderWidgetController() { Value = Value, MinimumValue = MinimumValue, MaximumValue = MaximumValue, Callback = Callback };
    }
}