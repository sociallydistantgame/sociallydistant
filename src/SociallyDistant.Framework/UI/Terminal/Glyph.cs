using Microsoft.Xna.Framework;

namespace SociallyDistant.Core.UI.Terminal;

public struct Glyph
{
    public char           character;
    public GlyphAttribute mode;
    public int            bg;
    public int            fg;
    public Color?         fgRgb;
    public Color?          bgRgb;
}