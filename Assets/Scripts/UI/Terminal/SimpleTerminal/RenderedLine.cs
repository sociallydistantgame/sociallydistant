using System;
using System.Text;
using TMPro;
using UI.Terminal.SimpleTerminal.Data;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Utility;
using Object = UnityEngine.Object;
using RectTransform = UnityEngine.RectTransform;

namespace UI.Terminal.SimpleTerminal
{
    public class RenderedLine
    {
        private TextPiece[] pieces = Array.Empty<TextPiece>();
        private readonly UguiTerminalScreen term;
        private RectTransform rootTransform;
        private GameObject root;
        private TextMeshProUGUI fg;
        private LayoutElement rootLayoutElement;
        private StringBuilder fsb;

        public Rect rect => this.rootTransform.rect;

        public RenderedLine(UguiTerminalScreen term)
        {
            this.term = term;

            this.BuildGui();
        }

        public void SetFont(TMP_FontAsset font, int fontSize)
        {
            this.fg.font = font;
            this.fg.fontSize = fontSize;
        }
        
        public void SetColumnSize(int cols)
        {
            int l = this.pieces.Length;
            Array.Resize(ref this.pieces, cols);
            for (int i = l; i < cols; i++)
            {
                ref TextPiece piece = ref this.pieces[i];
                piece.SetDirty();
            }
        }

        public void Destroy()
        {
            UnityEngine.Object.Destroy(root);
        }

        private void BuildGui()
        {
            // Create the root visual element that holds the foreground and background text.
            this.root = new GameObject("SimpleTerminal Line Object");
            this.rootTransform = root.AddComponent<RectTransform>();
            this.rootLayoutElement = root.AddComponent<LayoutElement>();

            // Create the Label elements for the background and foreground text.
            var fgGameObject = new GameObject("Foreground");
            this.fg = fgGameObject.AddComponent<TextMeshProUGUI>();

            // Prevent word-wrapping because it's handled by the terminal
            this.fg.enableWordWrapping = false;

            var layoutGroup = root.AddComponent<VerticalLayoutGroup>();
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.childForceExpandWidth = false;
            layoutGroup.childControlWidth = true;
            layoutGroup.childControlHeight = true;

            // Now assign the hierarchical hell
            // 
            // TERMINAL
            //   TEXT LINE
            //     BACKGROUND
            //     TEXT
            this.rootTransform.SetParent(this.term.transform);
            fg.transform.SetParent(rootTransform);

            rootLayoutElement.minHeight = this.term.UnscaledLineHeight;
        }

        public void SetGlyphs(ref Glyph[] glyphs, int start, int end, int y)
        {
            // Start by copying the bg and fg text into StringBuilders.
            // This is so that we can edit them without creating new strings
            // until we need to. Performance!
            this.fsb ??= new StringBuilder(this.fg.text ?? string.Empty);

            // Track if anything has actually changed
            var hasAnythingChanged = false;

            // Track where the change starts
            int fgDirt = 0;
            int fgClean = 0;
            int renderstart = -1;
            int lastPrintable = -1;

            // Now we must update the changed glyphs.
            for (int i = 0; i < pieces.Length; i++)
            {
                // Grab the glyph and corresponding piece by-ref.
                // Thank you modern C#
                ref TextPiece piece = ref this.pieces[i];
                ref Glyph glyph = ref glyphs[i];

                if (i >= 1)
                {
                    ref TextPiece prev = ref this.pieces[i - 1];
                    piece.BGStart = prev.BGStart + prev.BGLength;
                    piece.FGStart = prev.FGStart + prev.FGLength;
                }
                else
                {
                    piece.BGStart = 0;
                    piece.FGStart = 0;
                }

                bool reversed = (glyph.mode & GlyphAttribute.ATTR_REVERSE) != 0;
                if (this.term.Selected(i, y))
                    reversed = !reversed;

                int fgColorId = reversed ? glyph.bg : glyph.fg;
                int bgColorId = reversed ? glyph.fg : glyph.bg;

                Color rgbFgColor = reversed ? default : glyph.fgRgb;
                Color rgbBgColor = reversed ? glyph.fgRgb : default;

                
                if (i >= start && i <= end)
                {
                    piece.SetCharacter(glyph.character == 0 ? ' ' : glyph.character);
                    piece.SetBold((glyph.mode & GlyphAttribute.ATTR_BOLD) != 0);
                    piece.SetFaint((glyph.mode & GlyphAttribute.ATTR_FAINT) != 0);
                    piece.SetItalic((glyph.mode & GlyphAttribute.ATTR_ITALIC) != 0);
                    piece.SetUnderline((glyph.mode & GlyphAttribute.ATTR_UNDERLINE) != 0);
                    piece.SetStruck((glyph.mode & GlyphAttribute.ATTR_STRUCK) != 0);
                    
                    piece.isDefaultBG = this.term.DefaultBackgroundId == bgColorId;

                    string fgColor = this.term.GetColorOld(fgColorId, rgbFgColor);
                    string bgColor = this.term.GetColorOld(bgColorId, rgbBgColor);


                    piece.SetFGColor(fgColor);
                    piece.SetBGColor(bgColor);
                }

                bool isPrintable = glyph.character != '\0' && glyph.character != ' ';
                bool isBackgroundColored = bgColorId != term.DefaultBackgroundId;

                if (isPrintable || isBackgroundColored)
                    lastPrintable = i;
                
                if (piece.IsDirty)
                {
                    // first change
                    if (renderstart == -1)
                        renderstart = i;
                    
                    hasAnythingChanged = true;

                    fgDirt = fgClean;
                    
                    piece.BGLength = 0;
                    piece.FGLength = 0;
                }
                else if (hasAnythingChanged)
                {
                    // A previous glyph is dirty, therefore the current one must be marked as dirty.
                    piece.SetDirty();
                    piece.BGLength = 0;
                    piece.FGLength = 0;
                }
                else
                {
                    fgClean += piece.FGLength;
                }
            }
            
            
            // If nothing's changed, then do nothing.
            if (!hasAnythingChanged)
                return;

            // Nuke all the garbage
            fsb.Length = fgDirt;

            if (lastPrintable >= 0)
            {
                // Activate the text
                fg.gameObject.SetActive(true);

                // Now we tell each piece to rebuild.
                for (var i = renderstart; i <= lastPrintable; i++)
                {
                    bool isFirst = i == 0;
                    bool isLast = i == lastPrintable;

                    ref TextPiece current = ref this.pieces[i];

                    if (isFirst)
                    {
                        current.UpdateText(ref current, true, isLast, this.fsb);
                    }
                    else
                    {
                        ref TextPiece prev = ref this.pieces[i - 1];
                        current.UpdateText(ref prev, false, isLast, this.fsb);
                    }

                    if (isLast)
                        break;
                }
            }
            else
            {
                fg.gameObject.SetActive(false);                
            }
            
            rootLayoutElement.minHeight = this.term.UnscaledLineHeight;
            
            this.fg.SetText(this.fsb);
        }

        private struct TextPiece
        {
            private bool init;
            private int bg;
            private int fg;
            private int bgLen;
            private int fgLen;
            private bool dirty;
            private string bgColor;
            private string fgColor;
            private bool bold;
            private bool italic;
            private bool struck;
            private bool underline;
            private bool faint;
            private char character;

            public bool isDefaultBG;

            public bool IsDirty => this.dirty;

            public int FGStart
            {
                get => this.fg;
                set
                {
                    if (this.fg != value)
                    {
                        this.dirty = true;
                        this.fg = value;
                    }
                }
            }

            public int FGLength
            {
                get => this.fgLen;
                set => this.fgLen = value;
            }

            public int BGStart
            {
                get => this.bg;
                set
                {
                    if (this.bg != value)
                    {
                        this.dirty = true;
                        this.bg = value;
                    }
                }
            }

            public int BGLength
            {
                get => this.bgLen;
                set => this.bgLen = value;
            }

            public void SetDirty()
            {
                this.__init();
                this.dirty = true;
            }

            public void UpdateText(ref TextPiece previous, bool isFirst, bool isLast,StringBuilder fgBuilder)
            {
                this.__init();

                // Do nothing if we're not dirty
                if (!this.dirty)
                    return;

                if (this.character == '$') this.character = '$';

                this.dirty = false;

                // Remove all of our old text
                // bgBuilder.Remove(bg, bgLen);
                // fgBuilder.Remove(fg, fgLen);

                bool boldChanged = previous.bold != this.bold || isFirst;
                bool italicChanged = previous.italic != this.italic || isFirst;
                bool struckChanged = previous.struck != this.struck || isFirst;
                bool underlineChanged = previous.underline != this.underline || isFirst;
                bool fgChanged = previous.fgColor != this.fgColor || isFirst;
                bool bgChanged = previous.bgColor != this.bgColor || isFirst || (isLast && !this.isDefaultBG);
                bool faintChanged = previous.faint != this.faint;

                bool hasMarkupChanged = isFirst
                                        || boldChanged
                                        || italicChanged
                                        || underlineChanged
                                        || faintChanged
                                        || struckChanged
                                        || fgChanged;

                bool fuck = bgChanged && this.character == ' ';

                // If our markup has changed or our character isn't a space, we need to
                // check if our StringBuilders have enough length to insert our markup
                // and character. Otherwise we'll crash trying to insert.
                //
                // This is how we'll track our new length
                int fgLenOld = fgBuilder.Length;
                
                // Color
                if (fgChanged)
                {
                    fgBuilder.Append("<color=#");
                    fgBuilder.Append(this.fgColor);
                    fgBuilder.Append('>');
                }

                if (faintChanged)
                {
                    if (this.faint)
                        fgBuilder.Append("<alpha=#7f>");
                    else
                        fgBuilder.Append("<alpha=#ff>");
                }
                
                // Bold
                if (boldChanged)
                {
                    if (this.bold)
                        fgBuilder.Append("<b>");
                    else if (!isFirst)
                        fgBuilder.Append("</b>");
                }

                // Italic
                if (italicChanged)
                {
                    if (this.italic)
                        fgBuilder.Append("<i>");
                    else if (!isFirst)
                        fgBuilder.Append("</i>");
                }

                // Underline
                if (underlineChanged)
                {
                    if (this.underline)
                        fgBuilder.Append("<u>");
                    else if (!isFirst)
                        fgBuilder.Append("</u>");
                }


                // Struck
                if (struckChanged)
                {
                    if (this.struck)
                        fgBuilder.Append("<s>");
                    else if (!isFirst)
                        fgBuilder.Append("</s>");
                }
                
                // Insert the character itself.
                if (fuck)
                {
                    // We need to add an underscore here instead
                    // of a space, that way the background highlight shows correctly
                    fgBuilder.Append($"<color=#00000000>_<color=#{this.fgColor}>");
                }
                else
                {
                    if (isLast && !this.isDefaultBG && this.character == ' ')
                    {
                        fgBuilder.Append($"<color=#00000000>_<color=#{this.fgColor}>");
                    }
                    else
                    {
                        bool noparse = this.character is ('<' or '>');

                        if (noparse)
                            fgBuilder.Append("<noparse>");

                        fgBuilder.Append(this.character);

                        if (noparse)
                            fgBuilder.Append("</noparse>");
                    }
                }

                // Update our lengths
                this.fgLen = Math.Max(fgBuilder.Length - fgLenOld, 0);
            }

            public void SetBGColor(string new_bgColor)
            {
                this.__init();

                if (this.bgColor != new_bgColor)
                {
                    this.bgColor = new_bgColor;
                    this.dirty = true;
                }
            }

            public void SetFGColor(string new_fgColor)
            {
                this.__init();

                if (this.fgColor != new_fgColor)
                {
                    this.fgColor = new_fgColor;
                    this.dirty = true;
                }
            }

            public void SetBold(bool new_bold)
            {
                this.__init();

                if (this.bold != new_bold)
                {
                    this.bold = new_bold;
                    this.dirty = true;
                }
            }

            public void SetItalic(bool new_italic)
            {
                this.__init();

                if (this.italic != new_italic)
                {
                    this.italic = new_italic;
                    this.dirty = true;
                }
            }

            public void SetStruck(bool new_struck)
            {
                this.__init();

                if (this.struck != new_struck)
                {
                    this.struck = new_struck;
                    this.dirty = true;
                }
            }

            public void SetUnderline(bool new_underline)
            {
                this.__init();

                if (this.underline != new_underline)
                {
                    this.underline = new_underline;
                    this.dirty = true;
                }
            }

            public void SetFaint(bool new_faint)
            {
                this.__init();

                if (this.faint != new_faint)
                {
                    this.faint = new_faint;
                    this.dirty = true;
                }
            }

            public void SetCharacter(char new_character)
            {
                this.__init();

                if (this.character != new_character)
                {
                    this.character = new_character;
                    this.dirty = true;
                }
            }

            private void __init()
            {
                if (this.init)
                    return;

                this.init = true;
                this.dirty = true;
                this.character = ' ';
                this.bold = false;
                this.italic = false;
                this.underline = false;
                this.struck = false;
                this.faint = false;
                this.fg = 0;
                this.fgLen = 0;
                this.bg = 0;
                this.bgLen = 0;
                this.fgColor = null;
                this.bgColor = null;
            }
        }
    }
}