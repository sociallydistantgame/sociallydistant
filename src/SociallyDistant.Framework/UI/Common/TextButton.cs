using AcidicGUI.CustomProperties;
using AcidicGUI.Events;
using AcidicGUI.Layout;
using AcidicGUI.TextRendering;
using AcidicGUI.Widgets;

namespace SociallyDistant.Core.UI.Common;

public sealed class SimpleField : ContentWidget
{
    private readonly FlexPanel  root = new();
    private readonly TextWidget text = new();
    private readonly Box        slot = new();

    public string TExt
    {
        get => text.Text;
        set => text.Text = value;
    }

    public override Widget? Content
    {
        get => slot.Content;
        set => slot.Content = value;
    }

    public SimpleField()
    {
        text.VerticalAlignment = VerticalAlignment.Middle;
        slot.VerticalAlignment = VerticalAlignment.Middle;
        root.Direction = Direction.Horizontal;
        root.Spacing = 3;
        slot.GetCustomProperties<FlexPanelProperties>().Mode = FlexMode.Proportional;
        text.FontWeight = FontWeight.SemiBold;
        
        Children.Add(root);
        root.ChildWidgets.Add(text);
        root.ChildWidgets.Add(slot);
    }
}

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