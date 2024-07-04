using Microsoft.Xna.Framework;

namespace AcidicGUI.Events;

public sealed class MouseScrollEvent : MouseEvent
{
    public float ScrollDelta { get; }
    
    public MouseScrollEvent(Vector2 position, float scrollDelta) : base(position)
    {
        ScrollDelta = scrollDelta;
    }
}