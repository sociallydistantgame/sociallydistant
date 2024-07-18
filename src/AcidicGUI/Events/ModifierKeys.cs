namespace AcidicGUI.Events;

[Flags]
public enum ModifierKeys : byte
{
    None    = 0,
    Control = 1,
    Alt     = 2,
    Shift   = 4
}