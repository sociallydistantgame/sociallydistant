using Microsoft.Xna.Framework;

namespace AcidicGUI.Events;

public sealed class MouseMoveEvent : GuiEvent
{
    public Vector2 Position { get; }
    public Vector2 Movement { get; }

    public MouseMoveEvent(Vector2 position, Vector2 movement)
    {
        Position = position;
        Movement = movement;
    }
}