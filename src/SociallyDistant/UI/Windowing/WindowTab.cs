using AcidicGUI.Events;
using AcidicGUI.Layout;
using AcidicGUI.TextRendering;
using AcidicGUI.Widgets;
using SociallyDistant.Core.Shell;
using SociallyDistant.Core.Shell.Windowing;

namespace SociallyDistant.UI.Windowing;

public sealed class WindowTab : 
    Widget, 
    IWindowTab,
    IMouseClickHandler
{
    private readonly StackPanel stackPanel = new();
    private readonly TextWidget titleText = new();
    private readonly Button closeButton = new();
    private readonly Icon closeIcon = new();

    private bool active = false;

    public int TabIndex { get; set; }
    public Action<int>? ClickCallback { get; set; }
    
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

    public bool Closeable
    {
        get => closeButton.Visibility == Visibility.Visible;
        set
        {
            closeButton.Visibility = value
                ? Visibility.Visible
                : Visibility.Collapsed;
        }
    }
    
    public WindowTab()
    {
        Children.Add(stackPanel);
        stackPanel.ChildWidgets.Add(titleText);
        stackPanel.ChildWidgets.Add(closeButton);
        closeButton.Content = closeIcon;

        titleText.FontWeight = FontWeight.SemiBold;
        
        closeIcon.IconSize = 18;
        closeIcon.IconString = MaterialIcons.Close;

        stackPanel.Direction = Direction.Horizontal;
        stackPanel.Spacing = 3;
        stackPanel.Margin = 3;
    }

    public void OnMouseClick(MouseButtonEvent e)
    {
        if (e.Button != MouseButton.Left)
            return;

        e.Handle();
        ClickCallback?.Invoke(TabIndex);
    }
}