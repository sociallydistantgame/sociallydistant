using System;

namespace UI.Terminal.SimpleTerminal.Data
{
    [Flags]
    public enum SelectionMode : byte
    {
        SEL_IDLE = 0,
        SEL_EMPTY = 1,
        SEL_READY = 2
    }
}