using AcidicGUI.Events;
using AcidicGUI.Rendering;
using Microsoft.Xna.Framework;

namespace AcidicGUI.Widgets;

public sealed class Button : 
    ContentWidget,
    IMouseClickHandler
{
    public event Action? Clicked;
    
    public void OnMouseClick(MouseButtonEvent e)
    {
        if (e.Button != MouseButton.Left)
            return;

        e.Handle();
        Clicked?.Invoke();
    }
}