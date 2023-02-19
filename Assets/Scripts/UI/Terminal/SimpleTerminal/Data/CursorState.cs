using System;

namespace UI.Terminal.SimpleTerminal.Data
{
    [Flags]
    public enum CursorState : byte
    {
        CURSOR_DEFAULT = 0,
        CURSOR_WRAPNEXT = 1,
        CURSOR_ORIGIN = 2
    }
}