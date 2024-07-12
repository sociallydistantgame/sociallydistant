using AcidicGUI.ListAdapters;
using AcidicGUI.Widgets;
using Microsoft.Xna.Framework;

namespace SociallyDistant.Core.UI.Recycling;

public sealed class SliderWidgetController : RecyclableWidgetController
{
    private Slider? slider;
    
    public float MinimumValue { get; set; }
    public float MaximumValue { get; set; }
    public float Value { get; set; }
    public Action<float>? Callback { get; set; }
    public override void Build(ContentWidget destination)
    {
        slider = GetWidget<Slider>();

        slider.CurrentValue = (Value - MinimumValue) / (MaximumValue - MinimumValue);
        slider.ValueChanged += OnValueChanged;
        
        destination.Content = slider;
    }

    public override void Recycle()
    {
        if (slider != null)
            slider.ValueChanged -= OnValueChanged;
        
        Recyclewidget(slider);
        slider = null;
    }

    private void OnValueChanged(float newValue)
    {
        Callback?.Invoke(MathHelper.Lerp(MinimumValue, MaximumValue, newValue));
    }
}