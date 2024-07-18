using Microsoft.Xna.Framework.Input;
using Silk.NET.SDL;

namespace AcidicGUI.Events;

public class KeyEvent : GuiEvent
{
    public Keys Key { get; }
    public ModifierKeys Modifiers { get; }

    public KeyEvent(Keys key, ModifierKeys modifiers) 
    {
        Key = key;
        this.Modifiers = modifiers;
    }
}