using AcidicGUI.Widgets;
using SociallyDistant.Core.UI.VisualStyles;

namespace SociallyDistant.Core.UI.Recycling.SettingsWidgets;

public sealed class SectionTitle : Widget
{
    private readonly TextWidget text = new();

    public string Text
    {
        get => text.Text;
        set => text.Text = value;
    }

    public SectionTitle()
    {
        Children.Add(text);
        text.WordWrapping = false;
        text.UseMarkup = false;

        text.SetCustomProperty(WidgetForegrounds.SectionTitle);
    }
}