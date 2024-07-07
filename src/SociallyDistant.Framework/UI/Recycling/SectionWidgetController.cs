using AcidicGUI.ListAdapters;
using AcidicGUI.Widgets;
using SociallyDistant.Core.UI.Recycling.SettingsWidgets;

namespace SociallyDistant.Core.UI.Recycling;

public sealed class SectionWidgetController : RecyclableWidgetController
{
    private SectionTitle? sectionTitle;
    public string Text { get; set; } = string.Empty;
	
    public override void Build(ContentWidget destination)
    {
        sectionTitle = GetWidget<SectionTitle>();
        sectionTitle.Text = Text;
        destination.Content = sectionTitle;
    }

    public override void Recycle()
    {
        if (sectionTitle != null)
            Recyclewidget(sectionTitle);

        sectionTitle = null;
    }
}