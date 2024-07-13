using AcidicGUI.Events;
using AcidicGUI.Layout;
using AcidicGUI.Rendering;
using Microsoft.Xna.Framework;

namespace AcidicGUI.Widgets;

public sealed class Toggle : Widget,
    IMouseEnterHandler,
    IMouseLeaveHandler,
    IGainFocusHandler,
    ILoseFocusHandler,
    IMouseDownHandler,
    IMouseUpHandler,
    IMouseClickHandler
{
    private bool isSwitchVariant;
    private bool hovered;
    private bool pressed;
    private bool focused;
    private bool toggleValue;

    public event Action<bool>? OnValueChanged;

    public bool UseSwitchVariant
    {
        get => isSwitchVariant;
        set
        {
            isSwitchVariant = value;
            InvalidateLayout();
        }
    }
    
    public bool ToggleValue
    {
        get => toggleValue;
        set
        {
            if (this.toggleValue == value)
                return;
            
            this.toggleValue = value;
            NotifyChange();
            InvalidateGeometry();
        }
    }
    public bool IsHovered => hovered;
    public bool IsPressed => pressed;

    protected override Point GetContentSize(Point availableSize)
    {
        return isSwitchVariant
            ? GetVisualStyle().SwitchSize
            : GetVisualStyle().ToggleSize;
    }

    protected override void RebuildGeometry(GeometryHelper geometry)
    {
        var size = isSwitchVariant
            ? GetVisualStyle().SwitchSize
            : GetVisualStyle().ToggleSize;
        
        var rect = new LayoutRect(ContentArea.Left + ((ContentArea.Width - size.X) / 2), ContentArea.Top + ((ContentArea.Height - size.Y) / 2), size.X, size.Y);

        if (isSwitchVariant)
            GetVisualStyle().DrawToggleSwitch(this, geometry, rect, hovered, pressed, focused, toggleValue);
        else 
            GetVisualStyle().DrawToggle(this, geometry, rect, hovered, pressed, focused, toggleValue);
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

    public void OnFocusGained(FocusEvent e)
    {
        focused = true;
        InvalidateGeometry();
    }

    public void OnFocusLost(FocusEvent e)
    {
        focused = false;
        InvalidateGeometry();
    }

    public void OnMouseDown(MouseButtonEvent e)
    {
        if (e.Button != MouseButton.Left)
            return;

        e.RequestFocus();
        pressed = true;
        InvalidateGeometry();
    }

    public void OnMouseUp(MouseButtonEvent e)
    {
        if (e.Button != MouseButton.Left)
            return;

        pressed = false;
        InvalidateGeometry();
    }

    public void OnMouseClick(MouseButtonEvent e)
    {
        if (e.Button != MouseButton.Left)
            return;

        e.RequestFocus();

        if (!hovered)
            return;

        toggleValue = !toggleValue;
        InvalidateGeometry();
        NotifyChange();
    }

    private void NotifyChange()
    {
        OnValueChanged?.Invoke(this.toggleValue);
    }
}