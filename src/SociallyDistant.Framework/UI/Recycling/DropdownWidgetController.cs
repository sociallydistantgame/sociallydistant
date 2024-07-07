using AcidicGUI.ListAdapters;
using AcidicGUI.Widgets;

namespace SociallyDistant.Core.UI.Recycling;

public sealed class DropdownWidgetController : RecyclableWidgetController
{
    public string[] Choices { get; set; } = Array.Empty<string>();
    public int CurrentIndex { get; set; } = -1;
    public Action<int>? Callback { get; set; }
    public override void Build(ContentWidget destination)
    {
    }

    public override void Recycle()
    {
    }
}