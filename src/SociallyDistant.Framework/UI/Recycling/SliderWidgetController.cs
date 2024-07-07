using AcidicGUI.ListAdapters;
using AcidicGUI.Widgets;

namespace SociallyDistant.Core.UI.Recycling;

public sealed class SliderWidgetController : RecyclableWidgetController
{
    public float MinimumValue { get; set; }
    public float MaximumValue { get; set; }
    public float Value { get; set; }
    public Action<float>? Callback { get; set; }
    public override void Build(ContentWidget destination)
    {
    }

    public override void Recycle()
    {
    }
}