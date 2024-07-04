using Microsoft.Xna.Framework.Input;

namespace AcidicGUI.Events;

public class KeyEvent : GuiEvent
{
    public Keys Key { get; }

    public KeyEvent(Keys key)
    {
        Key = key;
    }
}