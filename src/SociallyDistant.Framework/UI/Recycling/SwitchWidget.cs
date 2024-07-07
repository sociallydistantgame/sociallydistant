using AcidicGUI.ListAdapters;

namespace SociallyDistant.Core.UI.Recycling;

public sealed class SwitchWidget : IWidget
{
    public bool IsActive { get; set; }
    public Action<bool>? Callback { get; set; }
    
    public RecyclableWidgetController Build()
    {
        return new SwitchWidgetController() { IsActive = IsActive, Callback = Callback };
    }
}