using AcidicGUI.Events;
using AcidicGUI.Widgets;

namespace SociallyDistant.Core.UI.Common;

public sealed class ListItem : ContentWidget, 
    IMouseClickHandler
{
    public Action? ClickCallback { get; set; }
    
    private bool isActive;

    public bool IsActive
    {
        get => isActive;
        set
        {
            isActive = value;
            InvalidateGeometry();
        }
    }

    public void OnMouseClick(MouseButtonEvent e)
    {
        if (e.Button != MouseButton.Left)
            return;

        e.RequestFocus();
        ClickCallback?.Invoke();
    }
}