using Microsoft.Xna.Framework;

namespace AcidicGUI.Events;

public sealed class MouseMoveEvent : MouseEvent
{
    public Vector2 Movement { get; }

    public MouseMoveEvent(Vector2 position, Vector2 movement) : base(position)
    {
        Movement = movement;
    }
}