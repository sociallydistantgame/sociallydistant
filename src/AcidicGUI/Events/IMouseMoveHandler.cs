namespace AcidicGUI.Events;

public interface IMouseMoveHandler : IMouseHandler
{
    void OnMouseMove(MouseMoveEvent e);
}