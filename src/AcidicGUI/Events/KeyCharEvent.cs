using Microsoft.Xna.Framework.Input;

namespace AcidicGUI.Events;

public sealed class KeyCharEvent : KeyEvent
{
    public char Character { get; }

    public KeyCharEvent(Keys key, ModifierKeys modifiers, char character) : base(key, modifiers)
    {
        Character = character;
    }
}