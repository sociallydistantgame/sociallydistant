using AcidicGUI.ListAdapters;
using AcidicGUI.Widgets;

namespace SociallyDistant.Core.UI.Recycling;

public sealed class InputFieldWidgetController : RecyclableWidgetController
{
    public string Value { get; set; } = string.Empty;
    public Action<string> Callback { get; set; }
    public override void Build(ContentWidget destination)
    {
    }

    public override void Recycle()
    {
    }
}