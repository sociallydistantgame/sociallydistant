using AcidicGUI.Events;
using AcidicGUI.Layout;
using AcidicGUI.Rendering;
using Microsoft.Xna.Framework;

namespace AcidicGUI.Widgets;

public sealed class Slider : Widget,
    IMouseEnterHandler,
    IMouseLeaveHandler,
    IDragStartHandler,
    IDragHandler,
    IDragEndHandler
{
    private bool      hovered;
    private bool      pressed;
    private float     currentValue;
    private Direction direction = Direction.Horizontal;

    public event Action<float>? ValueChanged;
    
    public Direction Direction
    {
        get => direction;
        set
        {
            direction = value;
            InvalidateLayout();
        }
    }
    
    public float CurrentValue
    {
        get => currentValue;
        set
        {
            currentValue = value;
            InvalidateGeometry();
        }
    }

    protected override Vector2 GetContentSize(Vector2 availableSize)
    {
        float thickness = GetVisualStyle().SliderThickness;

        return direction == Direction.Horizontal
            ? new Vector2(100,       thickness)
            : new Vector2(thickness, 100);
    }

    protected override void RebuildGeometry(GeometryHelper geometry)
    {
        GetVisualStyle().DrawSlider(this, geometry, hovered, pressed, direction == Direction.Vertical, currentValue);
    }

    private void SetValue(float newValue)
    {
        if (Math.Abs(currentValue - newValue) <= float.Epsilon)
            return;

        currentValue = newValue;
        ValueChanged?.Invoke(currentValue);
    }

    public void OnMouseEnter(MouseMoveEvent e)
    {
        hovered = true;
        InvalidateGeometry();
    }

    public void OnMouseLeave(MouseMoveEvent e)
    {
        hovered = false;
        InvalidateLayout();
    }

    public void OnDragStart(MouseButtonEvent e)
    {
        if (e.Button != MouseButton.Left)
            return;

        e.RequestFocus();
        pressed = true;
    }

    public void OnDrag(MouseButtonEvent e)
    {
        if (e.Button != MouseButton.Left)
            return;

        if (direction == Direction.Vertical)
        {
            SetValue(1 - MathHelper.Clamp(e.Position.Y - ContentArea.Top, 0, ContentArea.Height) / ContentArea.Height);
        }
        else
        {
            SetValue(MathHelper.Clamp(e.Position.X - ContentArea.Left, 0, ContentArea.Width) / ContentArea.Width);
        }
        
        InvalidateGeometry();
    }

    public void OnDragEnd(MouseButtonEvent e)
    {
        if (e.Button != MouseButton.Left)
            return;

        pressed = false;
        InvalidateGeometry();
    }
}