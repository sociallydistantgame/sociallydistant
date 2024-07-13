using AcidicGUI.CustomProperties;
using AcidicGUI.Layout;
using AcidicGUI.Widgets;
using Microsoft.Xna.Framework;
using SociallyDistant.Core.Shell;
using SociallyDistant.Core.Shell.Common;
using SociallyDistant.Core.Shell.Windowing;
using SociallyDistant.Core.UI.VisualStyles;
using SociallyDistant.UI.Common;

namespace SociallyDistant.UI.Windowing;

public class WindowDecoration : Widget
{
    private readonly FlexPanel           flexPanel = new();
    private readonly FlexPanel           titleBar  = new();
    private readonly Box                 borderBox = new();
    private readonly Box                 clientBox = new();
    private readonly WindowBase          window;
    private readonly WindowDragSurface   dragSurface;
    private readonly CompositeIconWidget titleIcon   = new();
    private readonly WindowTabList       tabList     = new();
    private readonly Button              closeButton = new();
    private readonly Icon                closeIcon   = new();
    
    private WindowHints hints;

    public WindowHints Hints
    {
        get => hints;
        set
        {
            hints = value;
            ApplyHints();
        }
    }

    public CompositeIcon Icon
    {
        get => titleIcon.Icon;
        set => titleIcon.Icon = value;
    }

    public bool CanClose
    {
        get => closeButton.Visibility == Visibility.Visible;
        set =>
            closeButton.Visibility = value
                ? Visibility.Visible
                : Visibility.Collapsed;
    }
    
    public Widget? Client
    {
        get => clientBox.Content;
        set => clientBox.Content = value;
    }

    public WindowTabList Tabs => tabList;

    public CommonColor Color
    {
        get => borderBox.GetCustomProperty<CommonColor>();
        set
        {
            borderBox.SetCustomProperty(value);
            tabList.Color = value;
        }
    }
    
    public WindowDecoration(WindowBase window)
    {
        closeButton.Visibility = Visibility.Collapsed;
        this.window = window;
        this.dragSurface = new WindowDragSurface((window));

        flexPanel.Spacing = -1;
        
        Children.Add(flexPanel);

        flexPanel.ChildWidgets.Add(dragSurface);
        flexPanel.ChildWidgets.Add(borderBox);

        dragSurface.Content = titleBar;

        titleBar.Direction = Direction.Horizontal;
        titleBar.Padding = new Padding(3, 0);
        titleBar.Spacing = 3;

        titleBar.ChildWidgets.Add(titleIcon);
        titleBar.ChildWidgets.Add(tabList);
        titleBar.ChildWidgets.Add(closeButton);

        closeIcon.IconSize = 16;
        closeButton.Content = closeIcon;
        closeIcon.IconString = MaterialIcons.Close;
        
        tabList.GetCustomProperties<FlexPanelProperties>().Mode = FlexMode.Proportional;
        
        borderBox.Margin = 1;
        borderBox.Content = clientBox;

        titleIcon.VerticalAlignment = VerticalAlignment.Bottom;
        titleIcon.Padding = new Padding(0, 1);
        
        borderBox.SetCustomProperty(WidgetBackgrounds.WindowBorder);
        borderBox.GetCustomProperties<FlexPanelProperties>().Mode = FlexMode.Proportional;

        tabList.VerticalAlignment = VerticalAlignment.Bottom;

        closeButton.Clicked += window.Close;
        
        ApplyHints();
    }

    private void ApplyHints()
    {
        if (hints.ClientRendersWindowBackground)
        {
            clientBox.SetCustomProperty(WidgetBackgrounds.None);
        }
        else
        {
            clientBox.SetCustomProperty(WidgetBackgrounds.WindowClient);
        }
    }
}