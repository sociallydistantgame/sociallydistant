namespace AcidicGUI.Events;

public interface IDragStartHandler : IMouseHandler
{
    void OnDragStart(MouseButtonEvent e);
}