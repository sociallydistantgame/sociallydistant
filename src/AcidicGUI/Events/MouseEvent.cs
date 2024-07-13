using Microsoft.Xna.Framework;

namespace AcidicGUI.Events;

public abstract class MouseEvent : GuiEvent
{
    public Point Position { get; }

    public MouseEvent(Point position)
    {
        this.Position = position;
    }
}