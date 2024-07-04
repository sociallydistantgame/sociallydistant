namespace AcidicGUI.Events;

public interface IMouseScrollHandler : IMouseHandler
{
    void OnMouseScroll(MouseScrollEvent e);
}