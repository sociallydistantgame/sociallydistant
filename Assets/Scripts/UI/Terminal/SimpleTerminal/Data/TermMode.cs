using System;

namespace UI.Terminal.SimpleTerminal.Data
{
    [Flags]
    public enum TermMode : byte
    {
        MODE_WRAP = 1 << 0,
        MODE_INSERT = 1 << 1,
        MODE_ALTSCREEN = 1 << 2,
        MODE_CRLF = 1 << 3,
        MODE_ECHO = 1 << 4,
        MODE_PRINT = 1 << 5,
        MODE_UTF8 = 1 << 6
    }
}