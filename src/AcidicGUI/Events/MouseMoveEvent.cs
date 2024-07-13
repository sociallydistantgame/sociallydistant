using Microsoft.Xna.Framework;

namespace AcidicGUI.Events;

public sealed class MouseMoveEvent : MouseEvent
{
    public Point Movement { get; }

    public MouseMoveEvent(Point position, Point movement) : base(position)
    {
        Movement = movement;
    }
}