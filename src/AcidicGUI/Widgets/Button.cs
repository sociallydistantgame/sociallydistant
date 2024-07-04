using AcidicGUI.Events;
using AcidicGUI.Rendering;
using Microsoft.Xna.Framework;

namespace AcidicGUI.Widgets;

public sealed class Button : 
    ContentWidget,
    IMouseEnterHandler,
    IMouseLeaveHandler
{
    private bool hovered;
    
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
}