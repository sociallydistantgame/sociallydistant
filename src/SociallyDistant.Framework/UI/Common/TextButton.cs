using AcidicGUI.Events;
using AcidicGUI.Layout;
using AcidicGUI.TextRendering;
using AcidicGUI.Widgets;
using Microsoft.Xna.Framework;

namespace SociallyDistant.Core.UI.Common;

public class TextButton : Widget,
    IMouseEnterHandler,
    IMouseLeaveHandler,
    IMouseDownHandler,
    IMouseUpHandler,
    IGainFocusHandler,
    ILoseFocusHandler
{
    private readonly Button     button     = new();
    private readonly TextWidget textWidget = new();
    private          bool       isHovered;
    private          bool       isPressed;

    public Action? ClickCallback { get; set; }

    public bool IsHovered => isHovered;
    public bool IsPressed => isPressed;
    
    public string Text
    {
        get => textWidget.Text;
        set => textWidget.Text = value;
    }
    
    public TextButton()
    {
        Children.Add(button);
        button.Content = textWidget;

        textWidget.Padding = new Padding(15, 0);
        
        //textWidget.HorizontalAlignment = HorizontalAlignment.Center;
        //textWidget.VerticalAlignment = VerticalAlignment.Middle;
        
        button.Clicked += HandleClicked;
        
        this.textWidget.FontWeight = FontWeight.SemiBold;
    }

    private void HandleClicked()
    {
        ClickCallback?.Invoke();
    }

    protected override Vector2 GetContentSize(Vector2 availableSize)
    {
        return base.GetContentSize(availableSize);
    }

    public void OnMouseEnter(MouseMoveEvent e)
    {
        isHovered = true;
        InvalidateGeometry();
    }

    public void OnMouseLeave(MouseMoveEvent e)
    {
        isPressed = false;
        isHovered = false;
        InvalidateGeometry();
    }

    public void OnMouseDown(MouseButtonEvent e)
    {
        if (e.Button != MouseButton.Left)
            return;

        isPressed = true;
        e.RequestFocus();
        InvalidateGeometry();
    }

    public void OnMouseUp(MouseButtonEvent e)
    {
        if (e.Button != MouseButton.Left)
            return;

        isPressed = false;
        InvalidateGeometry();
    }

    public void OnFocusGained(FocusEvent e)
    {
        InvalidateGeometry();
    }

    public void OnFocusLost(FocusEvent e)
    {
        InvalidateGeometry();
    }
}