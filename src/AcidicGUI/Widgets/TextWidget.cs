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

    private int           previousWrapWidth;
    private Color?        color;
    private FontInfo      font;
    private string        text         = string.Empty;
    private bool          useMarkup    = false;
    private bool          wordWrapping = false;
    private bool          showMarkup;
    private TextAlignment textAlignment;
    private int?          fontSize;
    private FontWeight    fontWeight = FontWeight.Normal;
    private int           selectionStart;
    private int           selectionLength;

    public FontWeight FontWeight
    {
        get => fontWeight;
        set
        {
            fontWeight = value;
            InvalidateMeasurements();
            InvalidateLayout();
        }
    }

    public bool HasSelection => selectionStart >= 0 && selectionStart + selectionLength <= text.Length;
    
    public Color? TextColor
    {
        get => color;
        set
        {
            color = value;
            InvalidateGeometry();
        }
    }
    
    public int? FontSize
    {
        get => fontSize;
        set
        {
            fontSize = value;
            InvalidateMeasurements();
            InvalidateLayout();
        }
    }
    
    public bool ShowMarkup
    {
        get => showMarkup;
        set
        {
            showMarkup = value;
            RebuildText();
            InvalidateLayout();
        }
    }
    
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
            RebuildText();
            InvalidateLayout();
        }
    }
    
    public string Text
    {
        get => text;
        set
        {
            selectionStart = -1;
            selectionLength = 0;
            text = value;
            RebuildText();
            InvalidateLayout();
        }
    }
    
    public FontInfo Font
    {
        get => font;
        set
        {
            font = value;
            InvalidateMeasurements();
            InvalidateLayout();
        }
    }

    protected override Point GetContentSize(Point availableSize)
    {
        int wrapWidth = availableSize.X;

        if (wrapWidth != previousWrapWidth)
        {
            InvalidateMeasurements();
            previousWrapWidth = wrapWidth;
        }

        // Measure text elements
        MeasureElements();

        Point result = Point.Zero;
        int lineHeight = 0;
        int lineWidth = 0;

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
        if (Math.Abs(previousWrapWidth - availableSpace.Width) > 0.001f)
        {
            InvalidateMeasurements();
            previousWrapWidth = availableSpace.Width;
            MeasureElements();
        }
        
        // Break words and figure out where lines start and end.
        var lines = BreakWords(availableSpace);

        var y = availableSpace.Top;
        foreach ((int start, int end, int lineWidth) in lines)
        {
            int lineHeight = 0;
            int offset = 0;
            int widgetX = availableSpace.Left;

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
                lineHeight = Math.Max(lineHeight, textElements[i].MeasuredSize?.Y ?? 0);
                int x = widgetX + offset;
                textElements[i].Position = new Point(x, y);
                offset += textElements[i].MeasuredSize?.X ?? 0;
            }

            y += lineHeight;
        }
    }

    public void SelectAll()
    {
        selectionStart = 0;
        selectionLength = text.Length;
        RebuildText();
        InvalidateLayout();
    }

    public void SelectNone()
    {
        selectionStart = -1;
        selectionLength = 0;
        RebuildText();
        InvalidateLayout();
    }

    public void SetSelection(int start, int count)
    {
        if (count == 0 && start==-1)
        {
            SelectNone();
            return;
        }

        if (start < 0)
            throw new ArgumentOutOfRangeException(nameof(start));
        
        if (count < 0)
            throw new ArgumentOutOfRangeException(nameof(count));
        
        if (start + count > text.Length)
            throw new ArgumentOutOfRangeException(nameof(count));

        selectionStart = start;
        selectionLength = count;
        RebuildText();
        InvalidateLayout();
    }
    
    protected override void RebuildGeometry(GeometryHelper geometry)
    {
        FontInfo? lastOverride = null;
        IFontFamily? family = null;
        
        foreach (TextElement element in textElements)
        {
            if (family == null || lastOverride != element.MarkupData.FontOverride)
            {
                family = (element.MarkupData.FontOverride ?? this.font).GetFont(this);
                lastOverride = element.MarkupData.FontOverride;
            }

            var renderColor = element.MarkupData.IsSelected
                ? GetVisualStyle().TextSelectionForeground
                : (element.MarkupData.ColorOverride ?? TextColor) ?? GetVisualStyle().GetTextColor(this);

            if (element.MeasuredSize.HasValue && element.MarkupData.Highlight.A > 0 || element.MarkupData.IsSelected)
            {
                var highlight = element.MarkupData.IsSelected
                    ? GetVisualStyle().TextSelectionBackground
                    : element.MarkupData.Highlight;
                
                var highlightRect = new LayoutRect(
                    element.Position.X,
                    element.Position.Y,
                    element.MeasuredSize.Value.X,
                    element.MeasuredSize.Value.Y
                );

                geometry.AddQuad(highlightRect, highlight);
            }

            family.Draw(geometry, element.Position.ToVector2(), renderColor, element.Text, element.MarkupData.FontSize ?? this.FontSize,
                element.MarkupData.Weight ?? FontWeight, element.MarkupData.Italic);

            var strikeLine = 1;
            var underLine = 2;

            if (element.MarkupData.Underline)
            {
                geometry.AddQuad(new LayoutRect(
                    element.Position.X,
                    element.Position.Y + element.MeasuredSize!.Value.Y - underLine,
                    element.MeasuredSize.Value.X,
                    underLine
                ), renderColor);
            }
            else if (!string.IsNullOrWhiteSpace(element.MarkupData.Link))
            {
                geometry.AddQuad(new LayoutRect(
                    element.Position.X,
                    element.Position.Y + element.MeasuredSize!.Value.Y - strikeLine,
                    element.MeasuredSize.Value.X,
                    strikeLine
                ), renderColor);
            }
            
            if (element.MarkupData.Strikethrough)
            {
                geometry.AddQuad(new LayoutRect(
                    element.Position.X,
                    element.Position.Y + ((element.MeasuredSize!.Value.Y - underLine)/2),
                    element.MeasuredSize.Value.X,
                    strikeLine
                ), renderColor);
            }
        }
    }

    private (int start, int end, int size)[] BreakWords(LayoutRect availableSpace)
    {
        var lines = new List<(int, int, int)>();
        int start = 0;
        int lineHeight = 0;
        Point offset = Point.Zero;

        for (var i = 0; i < textElements.Count; i++)
        {
            if (i == textElements.Count-1)
            {
                var family = (textElements[i].MarkupData.FontOverride ?? font).GetFont(this);
                var newMeasurement = family
                    .Measure(textElements[i].Text.TrimEnd(), textElements[i].MarkupData.FontSize ?? FontSize,
                        textElements[i].MarkupData.Weight ?? FontWeight, textElements[i].MarkupData.Italic);
                
                newMeasurement.Y = family.GetLineHeight(textElements[i].MarkupData.FontSize ?? FontSize, textElements[i].MarkupData.Weight ?? FontWeight, textElements[i].MarkupData.Italic);
                textElements[i].MeasuredSize = newMeasurement;
            }
            
            var measurement = textElements[i].MeasuredSize.GetValueOrDefault();
            var isNewLine = textElements[i].IsNewLine;
            var wrap = wordWrapping && (offset.X + measurement.X > availableSpace.Width) && availableSpace.Width > 0;
            
            if (isNewLine || wrap)
            {
                if (i > 0)
                {
                    offset.X -= textElements[i - 1].MeasuredSize!.Value.X;

                    var newFamily = (textElements[i - 1].MarkupData.FontOverride ?? font).GetFont(this);
                    
                    var newMeasurement = newFamily
                        .Measure(textElements[i - 1].Text.TrimEnd(),
                            textElements[i - 1].MarkupData.FontSize ?? FontSize,
                            textElements[i - 1].MarkupData.Weight ?? FontWeight,
                            textElements[i - 1].MarkupData.Italic);

                    newMeasurement.Y = newFamily.GetLineHeight(textElements[i-1].MarkupData.FontSize ?? FontSize, textElements[i-1].MarkupData.Weight ?? FontWeight, textElements[i-1].MarkupData.Italic);
                    textElements[i - 1].MeasuredSize = newMeasurement;

                    offset.X += newMeasurement.X;
                }

                lines.Add((start, i, offset.X));
                start = i;
                
                offset.X = 0;
                offset.Y += lineHeight;
                
                lineHeight = measurement.Y;
            }
            
            offset.X += measurement.X;
            lineHeight = Math.Max(lineHeight, measurement.Y);
            
            textElements[i].IsWrapPoint = wrap;
        }

        lines.Add((start, textElements.Count, offset.X));
        start = textElements.Count;
        
        return lines.ToArray();
    }

    public bool TryFindLink(Vector2 position, out string? linkId)
    {
        linkId = null;

        foreach (TextElement element in textElements)
        {
            if (element.MeasuredSize == null)
                continue;

            if (string.IsNullOrWhiteSpace(element.MarkupData.Link))
                continue;

            LayoutRect rect = new LayoutRect(
                element.Position.X,
                element.Position.Y,
                element.MeasuredSize.Value.X,
                element.MeasuredSize.Value.Y
            );

            if (!rect.Contains(position))
                continue;

            linkId = element.MarkupData.Link;
            return true;
        }

        return false;
    }
    
    private bool ParseMarkup(ReadOnlySpan<char> chars, int start, ref MarkupData markupData)
    {
        if (start < 0)
            return false;

        if (start >= chars.Length)
            return false;

        if (chars[start] != '<')
            return false;

        var end = start;
        
        for (var i = start; i <= chars.Length; i++)
        {
            if (i == chars.Length)
                return false;

            if (chars[i] == '>')
            {
                end = i + 1;
                break;
            }
        }

        var tag = chars.Slice(start, end - start);
        var tagWithoutAngles = tag.Slice(1, tag.Length - 2).ToString();

        markupData.Length = tag.Length;
        return ParseTag(tagWithoutAngles, ref markupData);
    }

    private bool ParseTag(string tag, ref MarkupData markupData)
    {
        var beforeEquals = tag;
        var afterEquals = string.Empty;

        var equalsIndex = tag.LastIndexOf("=", StringComparison.Ordinal);

        if (equalsIndex != -1)
        {
            beforeEquals = tag.Substring(0, equalsIndex);
            afterEquals = tag.Substring(equalsIndex + 1);
        }

        switch (beforeEquals)
        {
            case "size":
            {
                if (!int.TryParse(afterEquals, out int size) || size < 0)
                    return false;

                markupData.FontSize = size;
                return true;
            }
            case "/size":
            {
                markupData.FontSize = null;
                return true;
            }
            case "color":
            {
                if (ColorHelpers.ParseColor(afterEquals, out Color color))
                {
                    markupData.ColorOverride = color;
                    return true;
                }

                break;
            }
            case "/color":
            {
                markupData.ColorOverride = null;
                return true;
            }
            case "highlight":
            {
                if (ColorHelpers.ParseColor(afterEquals, out Color color))
                {
                    markupData.Highlight = color;
                    return true;
                }

                break;
            }
            case "/highlight":
            {
                markupData.Highlight = Color.Transparent;
                return true;
            }
            case "b":
                markupData.Weight = FontWeight.Bold;
                return true;
            case "/b":
                markupData.Weight = null;
                return true;
            case "i":
                markupData.Italic = true;
                return true;
            case "/i":
                markupData.Italic = false;
                return true;
            case "u":
                markupData.Underline = true;
                return true;
            case "/u":
                markupData.Underline = false;
                return true;
            case "s":
                markupData.Strikethrough = true;
                return true;
            case "/s":
                markupData.Strikethrough = false;
                return true;
            case "link":
            {
                if (string.IsNullOrWhiteSpace(afterEquals))
                    return false;

                markupData.Link = afterEquals;
                return true;
            }
            case "/link":
                markupData.Link = null;
                return true;
        }

        return false;
    }
    
    private void RebuildText()
    {
        var markupData = new MarkupData();
        var newMarkupData = new MarkupData();
        
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
                        SourceEnd = i,
                        MarkupData = markupData
                    });
                    markupData.Length = 0;
                    sourceStart = i;
                }

                stringBuilder.Length = 0;
                break;
            }

            if (i == selectionStart || i == selectionStart + selectionLength)
            {
                textElements.Add(new TextElement
                {
                    Text = stringBuilder.ToString(),
                    SourceStart = sourceStart,
                    SourceEnd = i,
                    MarkupData = markupData
                });

                markupData.IsSelected = !markupData.IsSelected;
                
                stringBuilder.Length = 0;
                sourceStart = i;
            }
            
            switch (character.Value)
            {
                case '<' when this.useMarkup:
                {
                    if (!ParseMarkup(chars, i, ref newMarkupData))
                    {
                        goto default;
                        break;
                    }

                    textElements.Add(new TextElement
                    {
                        Text = stringBuilder.ToString(),
                        SourceStart = sourceStart,
                        SourceEnd = i,
                        MarkupData = markupData
                    });

                    int markupLength = newMarkupData.Length;
                    
                    if (showMarkup)
                    {
                        textElements.Add(new TextElement
                        {
                            Text = chars.Slice(i, newMarkupData.Length).ToString(),
                            SourceStart = i,
                            SourceEnd = i + newMarkupData.Length,
                            MarkupData = markupData
                        });
                        
                        newMarkupData.Length = 0;
                    }
                    
                    markupData = newMarkupData;
                    
                    stringBuilder.Length = 0;
                    sourceStart = i;
                    
                    i += markupLength - 1;
                    break;
                }
                case '\r':
                    continue;
                case '\n':
                {
                    textElements.Add(new TextElement
                    {
                        Text = stringBuilder.ToString().TrimEnd(),
                        SourceStart = sourceStart,
                        SourceEnd = i,
                        MarkupData = markupData
                    });

                    markupData.Length = 0;
                    
                    stringBuilder.Length = 0;
                    sourceStart = i;
                    
                    textElements.Add(new TextElement
                    {
                        Text = stringBuilder.ToString(),
                        IsNewLine = true,
                        SourceStart = sourceStart,
                        SourceEnd = i + 1,
                        MarkupData = markupData
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
                            SourceEnd = i + 1,
                            MarkupData = markupData
                        });

                        markupData.Length = 0;
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
        FontInfo? lastOverride = null;
        IFontFamily? family = null;
        
        for (var i = 0; i < textElements.Count; i++)
        {
            if (textElements[i].MeasuredSize != null)
                continue;

            if (family == null || lastOverride != textElements[i].MarkupData.FontOverride)
            {
                family = (textElements[i].MarkupData.FontOverride ?? this.font).GetFont(this);
                lastOverride = textElements[i].MarkupData.FontOverride;
            }

            Point measurement = family.Measure(textElements[i].Text, textElements[i].MarkupData.FontSize ?? FontSize, textElements[i].MarkupData.Weight ?? FontWeight, textElements[i].MarkupData.Italic);
            measurement.Y = family.GetLineHeight(textElements[i].MarkupData.FontSize ?? FontSize, textElements[i].MarkupData.Weight ?? FontWeight, textElements[i].MarkupData.Italic);

            textElements[i].MeasuredSize = measurement;
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

            if (i == textElements.Count - 1 || characterIndex < element.SourceEnd)
            {
                var fontInstance = (element.MarkupData.FontOverride ?? font).GetFont(this);
                var lineHeight = fontInstance.GetLineHeight(element.MarkupData.FontSize ?? FontSize, element.MarkupData.Weight ?? FontWeight, element.MarkupData.Italic);
                
                if (characterIndex == element.SourceStart)
                {
                    string singleChar = element.Text.Substring(0, Math.Min(1, element.Text.Length));
                    Point charMeasure = fontInstance.Measure(singleChar, element.MarkupData.FontSize ?? FontSize, element.MarkupData.Weight ?? FontWeight, element.MarkupData.Italic);

                    return new LayoutRect(
                        element.Position.X,
                        element.Position.Y,
                        charMeasure.X,
                        lineHeight
                    );
                }

                string textToMeasure =
                    element.Text.Substring(0, Math.Min(element.Text.Length, characterIndex - element.SourceStart));
                
                Point measurement = fontInstance.Measure(textToMeasure, element.MarkupData.FontSize ?? FontSize, element.MarkupData.Weight ?? FontWeight, element.MarkupData.Italic);

                string charAfterMeasure =
                    element.Text.Substring(textToMeasure.Length, Math.Min(1, element.Text.Length - textToMeasure.Length));

                var singleCharMeasure = fontInstance.Measure(charAfterMeasure, element.MarkupData.FontSize ?? FontSize, element.MarkupData.Weight ?? FontWeight, element.MarkupData.Italic);

                return new LayoutRect(
                    element.Position.X + measurement.X,
                    element.Position.Y,
                    singleCharMeasure.X,
                    lineHeight
                );
            }
            
            i++;
        }
        
        Point lineMeasurement = font.GetFont(this).Measure(text);
        return new LayoutRect(
            ContentArea.Left,
            ContentArea.Top,
            lineMeasurement.X,
            lineMeasurement.Y
        );
    }

    private struct MarkupData
    {
        public int         Length;
        public Color?      ColorOverride;
        public Color       Highlight;
        public FontInfo?   FontOverride;
        public FontWeight? Weight;
        public int?        FontSize;
        public bool        Italic;
        public bool        Underline;
        public bool        Strikethrough;
        public string?     Link;
        public bool        IsSelected;
    }
    
    private class TextElement
    {
        public string     Text;
        public Point      Position;
        public Point?     MeasuredSize;
        public bool       IsNewLine;
        public bool       IsWrapPoint;
        public int        SourceStart;
        public int        SourceEnd;
        public MarkupData MarkupData = new();
    }
}