using System;

namespace UI.Terminal.SimpleTerminal.Data
{
    [Flags]
    public enum EscapeState : byte
    {
        ESC_START = 1,
        ESC_CSI = 2,
        ESC_STR = 4, /* DCS, OSC, PM, APC */
        ESC_ALTCHARSET = 8,
        ESC_STR_END = 16, /* a final string was encountered */
        ESC_TEST = 32, /* Enter in test mode */
        ESC_UTF8 = 64
    }
}