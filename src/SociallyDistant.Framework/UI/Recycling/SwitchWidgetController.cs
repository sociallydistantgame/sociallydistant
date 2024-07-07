using AcidicGUI.ListAdapters;
using AcidicGUI.Widgets;

namespace SociallyDistant.Core.UI.Recycling;

public sealed class SwitchWidgetController : RecyclableWidgetController
{
    public bool IsActive { get; set; }
    public Action<bool>? Callback { get; set; }


    public override void Build(ContentWidget destination)
    {
    }

    public override void Recycle()
    {
    }
}