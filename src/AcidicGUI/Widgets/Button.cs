using AcidicGUI.Events;
using AcidicGUI.Rendering;
using Microsoft.Xna.Framework;

namespace AcidicGUI.Widgets;

public sealed class Button : 
    ContentWidget,
    IMouseEnterHandler,
    IMouseClickHandler,
    IMouseLeaveHandler
{
    private bool hovered;

    public event Action? Clicked;
    
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

    protected override void RebuildGeometry(GeometryHelper geometry)
    {
        if (hovered)
        {
            geometry.AddQuad(ContentArea, Color.Red);
        }
    }

    public void OnMouseClick(MouseButtonEvent e)
    {
        if (e.Button != MouseButton.Left)
            return;

        e.Handle();
        Clicked?.Invoke();
    }
}