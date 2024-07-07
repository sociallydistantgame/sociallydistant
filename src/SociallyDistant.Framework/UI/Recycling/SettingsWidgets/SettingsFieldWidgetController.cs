using AcidicGUI.CustomProperties;
using AcidicGUI.ListAdapters;
using AcidicGUI.Widgets;

namespace SociallyDistant.Core.UI.Recycling.SettingsWidgets;

public sealed class SettingsFieldWidgetController : RecyclableWidgetController
{
    private SettingsField? field;
    
    public bool UseReverseLayout { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public RecyclableWidgetController? Slot { get; set; } = null;
	
    public override void Build(ContentWidget destination)
    {
        field = GetWidget<SettingsField>();
        field.Title = Title;
        field.Description = Description;

        Slot?.Build(field);

        destination.Content = field;
    }

    public override void Recycle()
    {
        if (field != null)
        {
            field.Content = null;
            Recyclewidget(field);
        }

        Slot?.Recycle();
    }
}