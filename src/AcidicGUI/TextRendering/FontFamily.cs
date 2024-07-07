using AcidicGUI.Rendering;
using Microsoft.Xna.Framework;

namespace AcidicGUI.TextRendering;

public sealed class FontFamily : IFontFamily
{
    private readonly Font regularFont;
    private readonly Font?[] weights = new Font?[Enum.GetValues(typeof(FontWeight)).Length];
    private readonly Font?[] italicWeights = new Font?[Enum.GetValues(typeof(FontWeight)).Length];

    public FontFamily(Font regularFont)
    {
        this.regularFont = regularFont;
    }

    public void SetFont(FontWeight weight, bool isItalic, Font? font)
    {
        if (isItalic)
            italicWeights[(int)weight] = font;
        else
            weights[(int)weight] = font;
    }
    
    public Font GetFont(FontWeight weight, bool italic)
    {
        Font? last = null;

        for (var i = 0; i < weights.Length; i++)
        {
            Font? font = weights[i];
            if (font == null)
                continue;

            if (italic)
            {
                Font? italicVariant = italicWeights[i];
                if (italicVariant != null)
                    return italicVariant;
            }

            last = font;
            if (i >= (int)weight)
                return last;
        }

        return last ?? regularFont;
    }

    public Vector2 Measure(string text, int? fontSize = null, FontWeight weight = FontWeight.Normal,
        bool preferItalic = false)
    {
        return GetFont(weight, preferItalic).Measure(text, fontSize);
    }

    public void Draw(GeometryHelper geometry, Vector2 position, Color color, string text, int? fontSize = null,
        FontWeight weight = FontWeight.Normal, bool preferItalic = false)
    {
        GetFont(weight, preferItalic).Draw(geometry,  position, color, text, fontSize);
    }
}