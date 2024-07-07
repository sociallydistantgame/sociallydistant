using System.Net.Mime;
using AcidicGUI.ListAdapters;
using AcidicGUI.Widgets;

namespace SociallyDistant.Core.UI.Recycling;

public sealed class LabelWidgetController : RecyclableWidgetController
{
    private TextWidget? text;
    
    public string LabelText { get; set; } = string.Empty;
    
    public override void Build(ContentWidget destination)
    {
        text = GetWidget<TextWidget>();

        text.WordWrapping = true;
        text.UseMarkup = true;
        text.Text = LabelText;

        destination.Content = text;
    }

    public override void Recycle()
    {
        Recyclewidget(text);
        text = null;
    }
}