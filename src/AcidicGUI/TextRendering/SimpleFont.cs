using System.Runtime.CompilerServices;
using AcidicGUI.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AcidicGUI.TextRendering;

internal sealed class SimpleFont : Font
{
    private readonly SpriteFont spriteFont;
    private readonly Dictionary<char, int> glyphIndices = new();
    private readonly int glyphCount;
    
    private Vector2 texCoordTL;
    private Vector2 texCoordBR;

    public SimpleFont(SpriteFont font)
    {
        this.spriteFont = font;
        this.glyphCount = font.Glyphs.Length;
        this.spriteFont.DefaultCharacter ??= '\ufffd';
    }

    public override Point Measure(string text, int? fontSize)
    {
        if (string.IsNullOrEmpty(text))
            return new Point(0, spriteFont.LineSpacing);
        
        return spriteFont.MeasureString(text).ToPoint();
    }

    public override unsafe void Draw(GeometryHelper geometryHelper, Vector2 position, Color color, string text, int? fontSize)
    {
        // TODO: Handle font sizes. Intention is to use scaling, even if it looks horrible.
        
        // This is basically SpriteBatch.DrawString() but without the SpriteBatch part.
        var mesh = geometryHelper.GetMeshBuilder(spriteFont.Texture);

        // Why is Texture2D.TexelWidth and Texture2D.TexelHeight internal?
        float texelWidth = 1f / spriteFont.Texture.Width;
        float texelHeight = 1f / spriteFont.Texture.Height;
        
        var offset = Vector2.Zero;
        var newline = true;
        
        fixed (SpriteFont.Glyph* glyphPtr = spriteFont.Glyphs)
        {
            for (var i = 0; i < text.Length; i++)
            {
                char character = text[i];

                switch (character)
                {
                    case '\r':
                        continue;
                    case '\n':
                        offset.X = 0;
                        offset.Y += (float) spriteFont.LineSpacing;
                        newline = true;
                        break;
                    default:
                        int glyphIndex = GetGlyphIndex(glyphPtr, character);
                        SpriteFont.Glyph* glyph = glyphPtr + glyphIndex;

                        if (newline)
                        {
                            offset.X = Math.Max(0, glyph->LeftSideBearing);
                            newline = false;
                        }
                        else
                        {
                            offset.X += spriteFont.Spacing + glyph->LeftSideBearing;
                        }
                        
                        this.texCoordTL.X = (float) glyph->BoundsInTexture.X * texelWidth;
                        this.texCoordTL.Y = (float) glyph->BoundsInTexture.Y * texelHeight;
                        this.texCoordBR.X = (float) (glyph->BoundsInTexture.X + glyph->BoundsInTexture.Width) * texelWidth;
                        this.texCoordBR.Y = (float) (glyph->BoundsInTexture.Y + glyph->BoundsInTexture.Height) * texelHeight;

                        Vector2 charPosition = position + offset;
                        
                        charPosition.X += (float) glyph->Cropping.X;
                        charPosition.Y += (float) glyph->Cropping.Y;

                        int v1 = mesh.AddVertex(charPosition, color, texCoordTL);
                        int v2 = mesh.AddVertex(new Vector2(charPosition.X + glyph->BoundsInTexture.Width, charPosition.Y), color,
                            new Vector2(texCoordBR.X, texCoordTL.Y));
                        int v3 = mesh.AddVertex(new Vector2(charPosition.X, charPosition.Y + glyph->BoundsInTexture.Height), color,
                            new Vector2(texCoordTL.X, texCoordBR.Y));
                        int v4 = mesh.AddVertex(new Vector2(charPosition.X + glyph->BoundsInTexture.Width, charPosition.Y + glyph->BoundsInTexture.Height), color,
                            new Vector2(texCoordBR.X, texCoordBR.Y));
                        
                        mesh.AddQuad(v1, v2, v3, v4);
                        
                        offset.X += glyph->Width + glyph->RightSideBearing;
                        break;
                }
            }
        }
    }

    public override int GetLineHeight(int? fontSizePixels)
    {
        return spriteFont.LineSpacing;
    }

    private unsafe int GetGlyphIndex(SpriteFont.Glyph* glyphs, char character)
    {
        if (!glyphIndices.TryGetValue(character, out int index))
        {
            var foundGlyph = false;

            for (var i = 0; i < glyphCount; i++)
            {
                SpriteFont.Glyph* glyph = glyphs + i;
                if (glyph->Character != character)
                    continue;

                foundGlyph = true;
                index = i;
                break;
            }
            
            if (foundGlyph)
                glyphIndices.Add(character, index);
            else
            {
                if (spriteFont.DefaultCharacter == null)
                    throw new ArgumentException($"Text contains a character that could not be resolved by the font.",
                        nameof(character));
                
                return GetGlyphIndex(glyphs, spriteFont.DefaultCharacter.Value);
            }
        }

        return index;
    }
}