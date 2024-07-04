using Microsoft.Xna.Framework;

namespace AcidicGUI.Events;

public abstract class MouseEvent : GuiEvent
{
    public Vector2 Position { get; }

    public MouseEvent(Vector2 position)
    {
        this.Position = position;
    }
}

public sealed class MouseMoveEvent : MouseEvent
{
    public Vector2 Movement { get; }

    public MouseMoveEvent(Vector2 position, Vector2 movement) : base(position)
    {
        Movement = movement;
    }
}