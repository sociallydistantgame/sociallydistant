namespace AcidicGUI.Events;

public interface IMouseClickHandler : IMouseHandler
{
    void OnMouseClick(MouseButtonEvent e);
}