using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AcidicGUI.Events;

public sealed class MouseButtonEvent : MouseEvent
{
    public MouseButton Button { get; }
    public ButtonState State { get; }
    public Point Movement { get; }

    public MouseButtonEvent(Point mousePosition, Point movement, MouseButton button, ButtonState state) :
        base(mousePosition)
    {
        Movement = movement;
        Button = button;
        State = state;
    }
}