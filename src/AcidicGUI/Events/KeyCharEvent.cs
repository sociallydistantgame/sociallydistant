using Microsoft.Xna.Framework.Input;

namespace AcidicGUI.Events;

public sealed class KeyCharEvent : KeyEvent
{
    public char Character { get; }

    public KeyCharEvent(Keys key, char character) : base(key)
    {
        Character = character;
    }
}