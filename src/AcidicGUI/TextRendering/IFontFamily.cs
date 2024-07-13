using AcidicGUI.Rendering;
using Microsoft.Xna.Framework;

namespace AcidicGUI.TextRendering;

public interface IFontFamily
{
    int GetLineHeight(int? fontSizePixels = null, FontWeight weight = FontWeight.Normal, bool preferItalic = false);
    
    Point Measure(
        string text,
        int? fontSize = null,
        FontWeight weight = FontWeight.Normal,
        bool preferItalic = false
    );

    void Draw(
        GeometryHelper geometry,
        Vector2 position,
        Color color,
        string text,
        int? fontSize = null,
        FontWeight weight = FontWeight.Normal,
        bool preferItalic = false
    );
}