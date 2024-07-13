using AcidicGUI.Events;
using AcidicGUI.Layout;
using AcidicGUI.Widgets;

namespace SociallyDistant.Core.UI.Common;

public sealed class ListItem : ContentWidget, 
    IMouseClickHandler,
    IMouseEnterHandler,
    IMouseLeaveHandler
{
    private readonly Box  contentBox = new();
    private          bool hovered;

    public override Widget? Content
    {
        get => contentBox.Content;
        set => contentBox.Content = value;
    }

    public Action? ClickCallback { get; set; }
    
    private bool isActive;

    public bool IsHovered => hovered;
    
    public bool IsActive
    {
        get => isActive;
        set
        {
            isActive = value;
            InvalidateGeometry();
        }
    }

    public ListItem()
    {
        Children.Add(contentBox);

        contentBox.Margin = new Padding(12, 6);
    }

    public void OnMouseClick(MouseButtonEvent e)
    {
        if (e.Button != MouseButton.Left)
            return;

        e.RequestFocus();
        ClickCallback?.Invoke();
    }

    public void OnMouseEnter(MouseMoveEvent e)
    {
        hovered = true;
        InvalidateGeometry();
    }

    public void OnMouseLeave(MouseMoveEvent e)
    {
        hovered = false;
        InvalidateGeometry();
    }
}