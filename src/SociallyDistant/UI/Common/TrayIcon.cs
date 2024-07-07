using AcidicGUI.Events;
using AcidicGUI.Widgets;
using SociallyDistant.Core.Shell;
using SociallyDistant.Core.UI.Shell;

namespace SociallyDistant.UI.Common;

public sealed class TrayIcon : Widget,
    IMouseClickHandler
{
    private readonly CompositeIconWidget iconWidget = new();

    private TrayAction? action;

    public TrayAction Action
    {
        get => action;
        set
        {
            action = value;
            UpdateIcon();
        }
    }

    public TrayIcon()
    {
        Children.Add(iconWidget);

        iconWidget.IconSize = 18;
    }

    private void UpdateIcon()
    {
        iconWidget.Icon = action?.Icon ?? MaterialIcons.Warning;
    }

    public void OnMouseClick(MouseButtonEvent e)
    {
        if (e.Button != MouseButton.Left)
            return;

        if (action == null)
            return;
        
        e.RequestFocus();
        action.Invoke();
    }
}