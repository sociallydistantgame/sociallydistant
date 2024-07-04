namespace AcidicGUI.Events;

public interface IMouseEnterHandler : IMouseHandler
{
    void OnMouseEnter(MouseMoveEvent e);
}