namespace AcidicGUI.Events;

public interface IDragEndHandler : IMouseHandler
{
    void OnDragEnd(MouseButtonEvent e);
}