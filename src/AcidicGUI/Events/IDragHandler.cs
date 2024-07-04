namespace AcidicGUI.Events;

public interface IDragHandler : IMouseHandler
{
    void OnDrag(MouseButtonEvent e);
}