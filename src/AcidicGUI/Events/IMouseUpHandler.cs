namespace AcidicGUI.Events;

public interface IMouseUpHandler : IMouseHandler
{
    void OnMouseUp(MouseButtonEvent e);
}