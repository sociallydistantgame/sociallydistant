using System;

namespace UI.Terminal.SimpleTerminal.Data
{
    [Flags]
    public enum GlyphAttribute : ushort
    {
        ATTR_NULL = 0,
        ATTR_SET = 1 << 0,
        ATTR_BOLD = 1 << 1,
        ATTR_FAINT = 1 << 2,
        ATTR_ITALIC = 1 << 3,
        ATTR_UNDERLINE = 1 << 4,
        ATTR_BLINK = 1 << 5,
        ATTR_REVERSE = 1 << 6,
        ATTR_INVISIBLE = 1 << 7,
        ATTR_STRUCK = 1 << 8,
        ATTR_WRAP = 1 << 9,
        ATTR_WIDE = 1 << 10,
        ATTR_WDUMMY = 1 << 11,
        ATTR_BOLD_FAINT = ATTR_BOLD | ATTR_FAINT
    }
}