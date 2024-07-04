using System.Diagnostics.Metrics;
using System.Formats.Tar;
using System.Text;
using System.Xml.XPath;
using AcidicGUI.Layout;
using AcidicGUI.Rendering;
using AcidicGUI.TextRendering;
using Microsoft.Xna.Framework;

namespace AcidicGUI.Widgets;

public class TextWidget : Widget
{
    private readonly List<TextElement> textElements = new();
    private readonly StringBuilder stringBuilder = new();
    
    private FontInfo font;
    private string text = string.Empty;
    private bool useMarkup = true;
    private bool wordWrapping = false;
    private TextAlignment textAlignment;

    public TextAlignment TextAlignment
    {
        get => textAlignment;
        set
        {
            textAlignment = value;
            InvalidateLayout();
        }
    }
    
    public bool WordWrapping
    {
        get => wordWrapping;
        set
        {
            wordWrapping = value;
            InvalidateLayout();
        }
    }
    
    public bool UseMarkup
    {
        get => useMarkup;
        set
        {
            useMarkup = value;
            InvalidateLayout();
            RebuildText();
        }
    }
    
    public string Text
    {
        get => text;
        set
        {
            text = value;
            InvalidateLayout();
            RebuildText();
        }
    }
    
    public FontInfo Font
    {
        get => font;
        set
        {
            font = value;
            InvalidateLayout();
            InvalidateMeasurements();
        }
    }

    protected override Vector2 GetContentSize(Vector2 availableSize)
    {
        float wrapWidth = availableSize.X;
        
        // Measure text elements
        MeasureElements();

        Vector2 result = Vector2.Zero;
        float lineHeight = 0;
        float lineWidth = 0;

        for (var i = 0; i < textElements.Count; i++)
        {
            var newline = textElements[i].IsNewLine;
            var measurement = textElements[i].MeasuredSize.GetValueOrDefault();
            var wrap = wordWrapping && (lineWidth + measurement.X > wrapWidth) && wrapWidth > 0;
            
            if (newline || wrap)
            {
                result.X = Math.Max(result.X, lineWidth);
                result.Y += lineHeight;
                lineHeight = 0;
                lineWidth = 0;
            }

            lineWidth += measurement.X;
            lineHeight = Math.Max(lineHeight, measurement.Y);
        }
        
        result.Y += lineHeight;
        result.X = Math.Max(result.X, lineWidth);

        return result;
    }

    protected override void ArrangeChildren(IGuiContext context, LayoutRect availableSpace)
    {
        // Break words and figure out where lines start and end.
        var lines = BreakWords(availableSpace);

        var y = availableSpace.Top;
        foreach ((int start, int end, float lineWidth) in lines)
        {
            float lineHeight = 0;
            float offset = 0;
            float widgetX = availableSpace.Left;

            if (textAlignment == TextAlignment.Center)
            {
                widgetX += (availableSpace.Width - lineWidth) / 2;
            }
            else if (textAlignment == TextAlignment.Right)
            {
                widgetX += availableSpace.Width - lineWidth;
            }
            
            for (var i = start; i < end; i++)
            {
                lineHeight = Math.Max(lineHeight, textElements[i].MeasuredSize!.Value.Y);
                float x = widgetX + offset;
                textElements[i].Position = new Vector2(x, y);
                offset += textElements[i].MeasuredSize!.Value.X;
            }

            y += lineHeight;
        }
    }

    protected override void RebuildGeometry(GeometryHelper geometry)
    {
        foreach (TextElement element in textElements)
        {
            var fontInstance = (element.FontOverride ?? font).GetFont(this);
            
            // TODO: Color from a property or the Visual Style.
            var color = (element.ColorOverride ?? Color.White);

            if (element.MeasuredSize.HasValue && element.Highlight.A > 0)
            {
                var highlightRect = new LayoutRect(
                    element.Position.X,
                    element.Position.Y,
                    element.MeasuredSize.Value.X,
                    element.MeasuredSize.Value.Y
                );

                geometry.AddQuad(highlightRect, element.Highlight);
            }
            
            fontInstance.Draw(geometry, element.Position, color, element.Text);
        }
    }

    private (int start, int end, float size)[] BreakWords(LayoutRect availableSpace)
    {
        var lines = new List<(int, int, float)>();
        int start = 0;
        float lineHeight = 0;
        Vector2 offset = Vector2.Zero;

        for (var i = 0; i < textElements.Count; i++)
        {
            if (i == textElements.Count-1)
            {
                textElements[i].MeasuredSize = (textElements[i].FontOverride ?? font).GetFont(this)
                    .Measure(textElements[i].Text.TrimEnd());
            }
            
            var measurement = textElements[i].MeasuredSize.GetValueOrDefault();
            var isNewLine = textElements[i].IsNewLine;
            var wrap = wordWrapping && (offset.X + measurement.X > availableSpace.Width);

            if (isNewLine || wrap)
            {
                if (i > 0)
                {
                    offset.X -= textElements[i - 1].MeasuredSize!.Value.X;
                    textElements[i - 1].MeasuredSize = (textElements[i - 1].FontOverride ?? font).GetFont(this)
                        .Measure(textElements[i - 1].Text.TrimEnd());
                    offset.X += textElements[i - 1].MeasuredSize!.Value.X;
                }

                lines.Add((start, i, offset.X));
                start = i;
                
                offset.X = 0;
                offset.Y += lineHeight;
                
                lineHeight = measurement.Y;
                textElements[i].IsNewLine = true;
            }

            offset.X += measurement.X;
            lineHeight = Math.Max(lineHeight, measurement.Y);
        }

        lines.Add((start, textElements.Count, offset.X));
        start = textElements.Count;
        
        return lines.ToArray();
    }
    
    private void RebuildText()
    {
        if (useMarkup)
        {
            RebuildTextWithMarkup();
        }
        else
        {
            RebuildTextWithoutMarkup();
        }
    }

    private void RebuildTextWithMarkup()
    {
        // TODO: Do this.
        RebuildTextWithoutMarkup();
    }

    private void RebuildTextWithoutMarkup()
    {
        var sourceStart = 0;
        
        textElements.Clear();

        ReadOnlySpan<char> chars = text.AsSpan();
        
        for (var i = 0; i <= chars.Length; i++)
        {
            char? character = i < chars.Length ? chars[i] : null;

            // End of text.
            if (!character.HasValue)
            {
                if (stringBuilder.Length > 0)
                {
                    textElements.Add(new TextElement
                    {
                        Text = stringBuilder.ToString().TrimEnd(),
                        SourceStart = sourceStart,
                        SourceEnd = i
                    });
                    sourceStart = i;
                }

                stringBuilder.Length = 0;
                break;
            }

            switch (character.Value)
            {
                case '\r':
                    continue;
                case '\n':
                {
                    textElements.Add(new TextElement
                    {
                        Text = stringBuilder.ToString().TrimEnd(),
                        SourceStart = sourceStart,
                        SourceEnd = i
                    });

                    stringBuilder.Length = 0;
                    sourceStart = i;
                    
                    textElements.Add(new TextElement
                    {
                        Text = stringBuilder.ToString(),
                        IsNewLine = true,
                        SourceStart = sourceStart,
                        SourceEnd = i + 1
                    });

                    sourceStart = i + 1;
                    break;
                }
                default:
                {
                    stringBuilder.Append(character.Value);
                    
                    if (char.IsWhiteSpace(character.Value))
                    {
                        textElements.Add(new TextElement
                        {
                            Text = stringBuilder.ToString(),
                            SourceStart = sourceStart,
                            SourceEnd = i + 1
                        });

                        sourceStart = i + 1;
                        stringBuilder.Length = 0;
                    }
                    break;
                }
            }
        }
    }

    public int GetLineCount()
    {
        return GetLineAtIndex(text.Length) + 1;
    }

    public int GetLineLength(int line)
    {
        var lineStart = GetLineStartElement(line);
        var length = 0;

        for (var i = lineStart; i < textElements.Count; i++)
        {
            if (textElements[i].IsNewLine)
                break;

            length += (textElements[i].SourceEnd - textElements[i].SourceStart);
        }

        return length;
    }
    
    public int GetLineStart(int line)
    {
        var lineStartElement = GetLineStartElement(line);
        return textElements[lineStartElement].SourceStart;
    }
    
    public int GetLineAtIndex(int characterIndex)
    {
        int line = 0;
        var wasNewLine = false;
        foreach (TextElement element in textElements)
        {
            if (element.SourceStart >= characterIndex)
                break;

            if (wasNewLine)
            {
                line++;
                wasNewLine = false;
            }

            if (element.IsNewLine)
                wasNewLine = true;
        }
     
        if (wasNewLine)
        {
            line++;
            wasNewLine = false;
        }
        
        return line;
    }
    
    public int GetLineStartElement(int line)
    {
        var currentLine = 0;
        var i = 0;
        var lineStartElement = 0;
        var wasNewLine = false;
        foreach (TextElement element in textElements)
        {
            if (currentLine == line)
                return lineStartElement;

            if (wasNewLine)
            {
                currentLine++;
                lineStartElement = i;
                wasNewLine = false;
            }

            if (element.IsNewLine)
                wasNewLine = true;

            i++;
        }
     
        if (wasNewLine)
        {
            currentLine++;
            wasNewLine = false;
            lineStartElement = textElements.Count - i;
        }
        
        return lineStartElement;
    }

    
    private void InvalidateMeasurements()
    {
        for (var i = 0; i < textElements.Count; i++)
        {
            textElements[i].MeasuredSize = null;
        }
    }
    
    private void MeasureElements()
    {
        for (var i = 0; i < textElements.Count; i++)
        {
            if (textElements[i].MeasuredSize != null)
                continue;
            
            var fontInstance = (textElements[i].FontOverride ?? font).GetFont(this);

            textElements[i].MeasuredSize = fontInstance.Measure(textElements[i].Text);
        }
    }

    public LayoutRect GetPositionOfCharacter(int characterIndex)
    {
        if (characterIndex < 0 || characterIndex > text.Length)
            throw new ArgumentOutOfRangeException(nameof(characterIndex));
        
        var i = 0;
        foreach (TextElement element in textElements)
        {
            if (characterIndex < element.SourceStart)
                break;

            if (i == textElements.Count - 1 || characterIndex <= element.SourceEnd)
            {
                var fontInstance = (element.FontOverride ?? font).GetFont(this);
                
                if (characterIndex == element.SourceStart)
                {
                    string singleChar = element.Text.Substring(0, Math.Min(1, element.Text.Length));
                    Vector2 charMeasure = fontInstance.Measure(singleChar);

                    return new LayoutRect(
                        element.Position.X,
                        element.Position.Y,
                        charMeasure.X,
                        charMeasure.Y
                    );
                }

                string textToMeasure =
                    element.Text.Substring(0, Math.Min(element.Text.Length, characterIndex - element.SourceStart));
                
                Vector2 measurement = fontInstance.Measure(textToMeasure);

                string charAfterMeasure =
                    element.Text.Substring(textToMeasure.Length, Math.Min(1, element.Text.Length - textToMeasure.Length));

                var singleCharMeasure = fontInstance.Measure(charAfterMeasure);

                return new LayoutRect(
                    element.Position.X + measurement.X,
                    element.Position.Y,
                    singleCharMeasure.X,
                    singleCharMeasure.Y
                );
            }
            
            i++;
        }
        
        Vector2 lineMeasurement = font.GetFont(this).Measure(text);
        return new LayoutRect(
            ContentArea.Left,
            ContentArea.Top,
            lineMeasurement.X,
            lineMeasurement.Y
        );
    }
    
    private class TextElement
    {
        public string Text;
        public Color? ColorOverride;
        public Color Highlight;
        public FontInfo? FontOverride;
        public Vector2 Position;
        public Vector2? MeasuredSize;
        public bool IsNewLine;
        public int SourceStart;
        public int SourceEnd;
    }
}