using UnityEngine;

namespace UI.Terminal.SimpleTerminal.Data
{
    public struct Glyph
    {
        public uint u;
        public GlyphAttribute mode;
        public int bg;
        public int fg;
        public Color fgRgb;
    }
}