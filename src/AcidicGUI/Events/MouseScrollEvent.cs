using Microsoft.Xna.Framework;

namespace AcidicGUI.Events;

public sealed class MouseScrollEvent : MouseEvent
{
    public int ScrollDelta { get; }
    
    public MouseScrollEvent(Point position, int scrollDelta) : base(position)
    {
        ScrollDelta = scrollDelta;
    }
}