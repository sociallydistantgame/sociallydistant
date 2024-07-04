namespace AcidicGUI.Events;

public interface IMouseDownHandler : IMouseHandler
{
    void OnMouseDown(MouseButtonEvent e);
}