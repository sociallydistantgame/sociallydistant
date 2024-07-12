using AcidicGUI.ListAdapters;
using AcidicGUI.Widgets;

namespace SociallyDistant.Core.UI.Recycling;

public sealed class SwitchWidgetController : RecyclableWidgetController
{
    private Toggle? toggle;
    
    public bool IsActive { get; set; }
    public Action<bool>? Callback { get; set; }


    public override void Build(ContentWidget destination)
    {
        toggle = GetWidget<Toggle>();

        toggle.ToggleValue = IsActive;
        toggle.OnValueChanged += HandleValueChanged;
        
        destination.Content = toggle;
    }

    private void HandleValueChanged(bool value)
    {
        this.Callback?.Invoke(value);
    }

    public override void Recycle()
    {
        if (toggle != null)
            toggle.OnValueChanged -= HandleValueChanged;

        this.Recyclewidget(toggle);
        toggle = null;
    }
}