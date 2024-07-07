using AcidicGUI.ListAdapters;

namespace SociallyDistant.Core.UI.Recycling;

public class SectionWidget : IWidget
{
    public string Text { get; set; } = string.Empty;
	
    public virtual RecyclableWidgetController Build()
    {
        return new SectionWidgetController { Text = Text };
    }
}