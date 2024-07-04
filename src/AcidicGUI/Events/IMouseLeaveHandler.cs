namespace AcidicGUI.Events;

public interface IMouseLeaveHandler : IMouseHandler
{
    void OnMouseLeave(MouseMoveEvent e);
}