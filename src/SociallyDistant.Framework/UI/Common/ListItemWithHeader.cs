using AcidicGUI.Layout;
using AcidicGUI.Widgets;
using SociallyDistant.Core.UI.Recycling.SettingsWidgets;

namespace SociallyDistant.Core.UI.Common;

public class ListItemWithHeader : Widget
{
    private readonly StackPanel stackPanel     = new();
    private readonly SectionTitle title          = new();
    private readonly ListItem   listItemWidget = new();
    private readonly TextWidget value          = new();

    public Action? ClickCallback
    {
        get => listItemWidget.ClickCallback;
        set => listItemWidget.ClickCallback = value;
    }
    
    public string Value
    {
        get => value.Text;
        set => this.value.Text = value;
    }
    
    public string Title
    {
        get => title.Text;
        set => title.Text = value;
    }

    public bool ShowTitle
    {
        get => title.Visibility == Visibility.Visible;
        set =>
            title.Visibility = value
                ? Visibility.Visible
                : Visibility.Collapsed;
    }

    public bool IsActive
    {
        get => listItemWidget.IsActive;
        set => listItemWidget.IsActive = value;
    }

    public ListItemWithHeader()
    {
        Children.Add(stackPanel);
        stackPanel.ChildWidgets.Add(title);
        stackPanel.ChildWidgets.Add(listItemWidget);
        listItemWidget.Content = value;

        title.Padding = new Padding(12, 6, 12, 3);
    }
}