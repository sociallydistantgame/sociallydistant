using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AcidicGUI.Events;

public sealed class MouseButtonEvent : MouseEvent
{
    public MouseButton Button { get; }
    public ButtonState State { get; }
    public Vector2 Movement { get; }

    public MouseButtonEvent(Vector2 mousePosition, Vector2 movement, MouseButton button, ButtonState state) :
        base(mousePosition)
    {
        Movement = movement;
        Button = button;
        State = state;
    }
}