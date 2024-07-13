using AcidicGUI.Events;
using AcidicGUI.Widgets;
using Microsoft.Xna.Framework;

namespace SociallyDistant.UI.Windowing;

public sealed class WindowDragSurface : 
    ContentWidget,
    IDragStartHandler,
    IDragHandler,
    IDragEndHandler
{
    private readonly WindowBase window;

    private bool dragging;
    
    public WindowDragSurface(WindowBase window)
    {
        MinimumSize = new Point(0, 24);
        this.window = window;
    }
    
    public void OnDragStart(MouseButtonEvent e)
    {
        if (e.Button != MouseButton.Left)
            return;

        e.RequestFocus();
        dragging = true;
    }

    public void OnDrag(MouseButtonEvent e)
    {
        if (e.Button != MouseButton.Left)
            return;
        
        if (!dragging)
            return;

        var settings = window.GetCustomProperties<WindowSettings>();

        if (settings.Maximized)
            return;

        settings.Position += e.Movement;

        e.RequestFocus();
    }

    public void OnDragEnd(MouseButtonEvent e)
    {
        if (e.Button != MouseButton.Left)
            return;
        
        if (!dragging)
            return;

        if (e.Button != MouseButton.Left)
            return;

        dragging = false;
    }
}