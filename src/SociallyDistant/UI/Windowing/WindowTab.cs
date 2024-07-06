using AcidicGUI.Layout;
using AcidicGUI.Widgets;
using SociallyDistant.Core.Shell;
using SociallyDistant.Core.Shell.Windowing;

namespace SociallyDistant.UI.Windowing;

public sealed class WindowTab : 
    Widget, 
    IWindowTab
{
    private readonly StackPanel stackPanel = new();
    private readonly TextWidget titleText = new();
    private readonly Button closeButton = new();
    private readonly Icon closeIcon = new();

    private bool active = false;

    public string Title
    {
        get => titleText.Text;
        set => titleText.Text = value;
    }

    public bool Active
    {
        get => active;
        set
        {
            active = value;
            InvalidateGeometry();
        }
    }
    
    public WindowTab()
    {
        Children.Add(stackPanel);
        stackPanel.ChildWidgets.Add(titleText);
        stackPanel.ChildWidgets.Add(closeButton);
        closeButton.Content = closeIcon;

        closeIcon.IconSize = 18;
        closeIcon.IconString = MaterialIcons.Close;

        stackPanel.Direction = Direction.Horizontal;
        stackPanel.Spacing = 3;
        stackPanel.Margin = 3;
    }
}