using System;

namespace UI.Terminal.SimpleTerminal.Data
{
    [Flags]
    public enum SelectionSnap : byte
    {
        NONE = 0,
        SNAP_WORD = 1,
        SNAP_LINE = 2
    }
}