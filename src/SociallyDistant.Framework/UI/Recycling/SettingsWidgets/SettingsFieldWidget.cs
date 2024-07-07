using AcidicGUI.ListAdapters;

namespace SociallyDistant.Core.UI.Recycling.SettingsWidgets;

public sealed class SettingsFieldWidget : IWidget
{
    public bool UseReverseLayout { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public IWidget? Slot { get; set; }

    public RecyclableWidgetController Build()
    {
        return new SettingsFieldWidgetController { Title = Title, Description = Description, Slot = Slot?.Build(), UseReverseLayout = UseReverseLayout };
    }
}