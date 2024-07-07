using AcidicGUI.ListAdapters;

namespace SociallyDistant.Core.UI.Recycling;

public sealed class DropdownWidget : IWidget
{
    public string[] Choices { get; set; } = Array.Empty<string>();
    public int CurrentIndex { get; set; } = -1;
    public Action<int>? Callback { get; set; }
    public RecyclableWidgetController Build()
    {
        return new DropdownWidgetController() { Choices = Choices, Callback = Callback, CurrentIndex = CurrentIndex };
    }
}