using AcidicGUI.CustomProperties;
using AcidicGUI.Layout;
using AcidicGUI.TextRendering;
using AcidicGUI.Widgets;
using SociallyDistant.Core.UI.VisualStyles;

namespace SociallyDistant.Core.UI.Recycling.SettingsWidgets;

public sealed class SettingsField : ContentWidget
{
    private readonly FlexPanel  root        = new();
    private readonly StackPanel infoSection = new();
    private readonly TextWidget title       = new();
    private readonly TextWidget description = new();
    private readonly Box        slot        = new();

    public string Title
    {
        get => title.Text;
        set => title.Text = value;
    }

    public string Description
    {
        get => description.Text;
        set => description.Text = value;
    }
    
    public override Widget? Content
    {
        get => slot.Content;
        set => slot.Content = value;
    }
    
    public SettingsField()
    {
        Children.Add(root);
        root.ChildWidgets.Add(infoSection);
        root.ChildWidgets.Add(slot);
        infoSection.ChildWidgets.Add(title);
        infoSection.ChildWidgets.Add(description);

        root.Direction = Direction.Horizontal;
        root.Spacing = 10;
        root.Padding = 10;

        infoSection.GetCustomProperties<FlexPanelProperties>().Mode = FlexMode.Proportional;

        description.WordWrapping = true;

        title.FontWeight = FontWeight.Bold;
        
        this.SetCustomProperty(WidgetBackgrounds.FormField);
    }
}