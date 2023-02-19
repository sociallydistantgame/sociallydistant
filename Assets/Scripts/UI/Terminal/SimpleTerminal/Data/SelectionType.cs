using System;

namespace UI.Terminal.SimpleTerminal.Data
{
    [Flags]
    public enum SelectionType : byte
    {
        SEL_REGULAR = 1,
        SEL_RECTANGULAR = 2
    }
}