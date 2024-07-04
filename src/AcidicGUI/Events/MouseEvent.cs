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