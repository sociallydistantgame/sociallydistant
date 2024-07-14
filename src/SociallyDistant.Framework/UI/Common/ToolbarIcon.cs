using AcidicGUI.Widgets;
using SociallyDistant.Core.Shell.Common;

namespace SociallyDistant.Core.UI.Common;

public class ToolbarIcon : Widget
{
    private readonly CompositeIconWidget icon   = new();
    private readonly Button              button = new();

    public CompositeIcon Icon
    {
        get => icon.Icon;
        set => icon.Icon = value;
    }
    
    public Action? ClickHandler { get; set; }
    
    public ToolbarIcon()
    {
        Children.Add(button);
        icon.IconSize = 16;
        button.Content = icon;
        button.Clicked += OnClick;
    }

    private void OnClick()
    {
        ClickHandler?.Invoke();
    }
}