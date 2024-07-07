using AcidicGUI.ListAdapters;

namespace SociallyDistant.Core.UI.Recycling;

public sealed class LabelWidget : IWidget
{
    public string Text { get; set; } = string.Empty;
    
    public RecyclableWidgetController Build()
    {
        return new LabelWidgetController { LabelText = Text };
    }
}