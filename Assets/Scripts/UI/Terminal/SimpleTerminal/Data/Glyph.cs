using UnityEngine;

namespace UI.Terminal.SimpleTerminal.Data
{
    public struct Glyph
    {
        public char character;
        public GlyphAttribute mode;
        public int bg;
        public int fg;
        public Color fgRgb;
    }
}