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
        private readonly SimpleTerminalRenderer term;
        private RectTransform rootTransform;
        private GameObject root;
        private TextMeshProUGUI bg;
        private TextMeshProUGUI fg;
        private StringBuilder bsb;
        private StringBuilder fsb;

        public Rect rect => this.rootTransform.rect;

        public RenderedLine(SimpleTerminalRenderer term)
        {
            this.term = term;

            this.BuildGui();
        }

        public void SetFont(TMP_FontAsset font, int fontSize)
        {
            this.bg.font = font;
            this.bg.fontSize = fontSize;
            this.fg.font = font;
            this.fg.fontSize = fontSize;
        }
        
        public void SwapWith(RenderedLine other)
        {
            TextMeshProUGUI otherfg = other.fg;
            TextMeshProUGUI otherbg = other.bg;
            TextPiece[] otherpieces = other.pieces;
            StringBuilder otherbsb = other.bsb;
            StringBuilder otherfsb = other.fsb;

            // Take possession of the other line's text
            otherbg.transform.SetParent(rootTransform);
            otherfg.transform.SetParent(rootTransform);

            // Give our own text to the other line
            this.bg.transform.SetParent(other.rootTransform);
            this.fg.transform.SetParent(other.rootTransform);
            
            // Give the other line our current references
            other.bg = this.bg;
            other.fg = this.fg;
            other.pieces = this.pieces;
            other.bsb = this.bsb;
            other.fsb = this.fsb;

            // Take the other line's data
            this.bg = otherbg;
            this.fg = otherfg;
            this.pieces = otherpieces;
            this.bsb = other.bsb;
            this.fsb = otherfsb;
        }

        public void CopyFrom(RenderedLine other)
        {
            Array.Copy(other.pieces, 0, this.pieces, 0, this.pieces.Length);

            this.bg.text = other.bg.text;
            this.fg.text = other.fg.text;

            this.bsb = new StringBuilder(this.bg.text);
            this.fsb = new StringBuilder(this.fg.text);
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

            // Create the Label elements for the background and foreground text.
            var bgGameObject = new GameObject("Background");
            var fgGameObject = new GameObject("Foreground");
            this.fg = fgGameObject.AddComponent<TextMeshProUGUI>();
            this.bg = bgGameObject.AddComponent<TextMeshProUGUI>();

            var bgLayoutElement = bgGameObject.AddComponent<LayoutElement>();
            bgLayoutElement.ignoreLayout = true;

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
            bg.transform.SetParent(rootTransform);
            fg.transform.SetParent(rootTransform);

            RectTransform bgRect = bg.transform as RectTransform;
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.pivot = Vector2.zero;
            bgRect.anchoredPosition = Vector3.zero;
            bgRect.sizeDelta = Vector2.zero;
        }

        public void SetGlyphs(ref Glyph[] glyphs, int start, int end, int y)
        {
            // Start by copying the bg and fg text into StringBuilders.
            // This is so that we can edit them without creating new strings
            // until we need to. Performance!
            this.bsb ??= new StringBuilder(this.bg.text ?? string.Empty);
            this.fsb ??= new StringBuilder(this.fg.text ?? string.Empty);

            // Track if anything has actually changed
            var hasAnythingChanged = false;

            // Now we must update the changed glyphs.
            int bgDirtStart = -1;
            int fgDirtStart = -1;
            int bgDirtCount = -1;
            var fgDirtCount = 0;
            var bgRemoved = 0;
            var fgRemoved = 0;
            var bgAdjust = 0;
            var fgAdjust = 0;
            for (int i = start; i < end && i < pieces.Length; i++)
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

                piece.SetCharacter(glyph.u == 0 ? ' ' : (char)glyph.u);
                piece.SetBold((glyph.mode & GlyphAttribute.ATTR_BOLD) != 0);
                piece.SetFaint((glyph.mode & GlyphAttribute.ATTR_FAINT) != 0);
                piece.SetItalic((glyph.mode & GlyphAttribute.ATTR_ITALIC) != 0);
                piece.SetUnderline((glyph.mode & GlyphAttribute.ATTR_UNDERLINE) != 0);
                piece.SetStruck((glyph.mode & GlyphAttribute.ATTR_STRUCK) != 0);

                if (glyph.u == '$')
                    glyph.u = '$';

                bool reversed = (glyph.mode & GlyphAttribute.ATTR_REVERSE) != 0;

                if (this.term.Selected(i, y))
                    reversed = !reversed;

                int fgColorId = reversed ? glyph.bg : glyph.fg;
                int bgColorId = reversed ? glyph.fg : glyph.bg;

                Color rgbFgColor = reversed ? default : glyph.fgRgb;
                Color rgbBgColor = reversed ? glyph.fgRgb : default;

                piece.isDefaultBG = this.term.DefaultBackgroundId == bgColorId;

                string fgColor = this.term.GetColor(fgColorId, rgbFgColor);
                string bgColor = this.term.GetColor(bgColorId, rgbBgColor);


                piece.SetFGColor(fgColor);
                piece.SetBGColor(bgColor);

                if (piece.IsDirty)
                {
                    hasAnythingChanged = true;

                    if (bgDirtStart < 0) bgDirtStart = piece.BGStart;

                    if (fgDirtStart < 0)
                        fgDirtStart = piece.FGStart;

                    bgDirtCount += piece.BGLength;
                    fgDirtCount += piece.FGLength;

                    piece.BGLength = 0;
                    piece.FGLength = 0;
                    piece.BGStart = bgDirtStart;
                    piece.FGStart = fgDirtStart;
                }
                else
                {
                    // If only the exact start of the line is dirty, we can
                    // optimize this whole thing by simply decreasing the length
                    // of the string builders.
                    //
                    // But if the middle of the string is dirty, we have to remove
                    // ranges of text from the string builders. Otherwise it'll corrupt
                    // the whole string.

                    var shouldBeDirty = false;
                    if (bgDirtStart >= 0)
                    {
                        // if (bgDirtStart == 0)
                        //     bsb.Length -= bgDirtCount;
                        // else
                        if (bgDirtCount > 0)
                        {
                            bgDirtCount += piece.BGLength;
                            int len = Math.Min(this.bsb.Length - bgDirtStart, bgDirtCount);
                            if (this.bsb.Length > bgDirtStart && len > 0) this.bsb.Remove(bgDirtStart, len);
                            shouldBeDirty = true;
                            piece.BGLength = 0;
                        }

                        piece.BGStart = bgDirtStart;

                        bgRemoved += bgDirtCount;
                        bgDirtCount = 0;
                        bgDirtStart = -1;
                    }

                    if (fgDirtStart >= 0)
                    {
                        // if (fgDirtStart == 0)
                        //     fsb.Length -= fgDirtCount;
                        // else
                        if (fgDirtCount > 0)
                        {
                            fgDirtCount += piece.FGLength;
                            int len = Math.Min(this.fsb.Length - fgDirtStart, fgDirtCount);
                            if (this.fsb.Length > fgDirtStart && len > 0) this.fsb.Remove(fgDirtStart, len);
                            shouldBeDirty = true;
                            piece.FGLength = 0;
                        }

                        piece.FGStart = fgDirtStart;

                        fgRemoved += fgDirtCount;
                        fgDirtCount = 0;
                        fgDirtStart = -1;
                    }

                    if (shouldBeDirty)
                        piece.SetDirty();

                    bgAdjust += piece.BGLength;
                    fgAdjust += piece.FGLength;
                }
            }

            // Situations where there's dirtyness that hasn't been cleared
            if (fgDirtStart >= 0 || bgDirtStart >= 0)
            {
                if (bgDirtStart >= 0)
                {
                    if (this.bsb.Length > bgDirtStart) this.bsb.Length = bgDirtStart;

                    bgRemoved += bgDirtCount;
                    bgDirtCount = 0;
                    bgDirtStart = -1;
                }

                if (fgDirtStart >= 0)
                {
                    if (this.fsb.Length > fgDirtStart) this.fsb.Length = fgDirtStart;

                    fgRemoved += fgDirtCount;
                    fgDirtCount = 0;
                    fgDirtStart = -1;
                }
            }

            // If nothing's changed, then do nothing.
            if (!hasAnythingChanged)
                return;

            // Now we tell each piece to rebuild.
            for (var i = 0; i < this.pieces.Length; i++)
            {
                bool isFirst = i == 0;
                bool isLast = i == this.pieces.Length - 1;

                ref TextPiece current = ref this.pieces[i];

                if (isFirst)
                {
                    current.UpdateText(ref current, true, isLast, this.bsb, this.fsb);
                }
                else
                {
                    ref TextPiece prev = ref this.pieces[i - 1];
                    current.UpdateText(ref prev, false, isLast, this.bsb, this.fsb);
                }
            }

            this.bsb.TrimEnd();
            this.fsb.TrimEnd();

            ref TextPiece last = ref this.pieces[this.pieces.Length - 1];
            Assert.IsFalse(last.BGStart + last.BGLength < this.bsb.Length);
            Assert.IsFalse(last.FGStart + last.FGLength < this.fsb.Length);

            this.bg.text = this.bsb.ToString();
            this.fg.text = this.fsb.ToString();
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

            public void UpdateText(ref TextPiece previous, bool isFirst, bool isLast, StringBuilder bgBuilder,
                StringBuilder fgBuilder)
            {
                this.__init();

                // Check if the end position of the previous piece doesn't line up with our start.
                // If that's happened, we still need to update our start position but do not need
                // to modify the actual text.
                if (!isFirst)
                {
                    int bgEnd = previous.bg + previous.bgLen;
                    int fgEnd = previous.fg + previous.fgLen;

                    if (this.bg != bgEnd)
                    {
                        this.bg = bgEnd;
                        this.dirty = true;

                        if (this.bgLen > 0 && bgBuilder.Length > this.bg + this.bgLen)
                        {
                            bgBuilder.Remove(this.bg, this.bgLen);
                            this.bgLen = 0;
                        }
                    }

                    if (this.fg != fgEnd)
                    {
                        this.fg = fgEnd;
                        this.dirty = true;

                        if (this.fgLen > 0 && fgBuilder.Length > this.fg + this.fgLen)
                        {
                            fgBuilder.Remove(this.fg, this.fgLen);
                            this.fgLen = 0;
                        }
                    }
                }

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
                // This is part of an optimization that trims excess whitespace off the
                // finished text before submitting to TMP.
                if (bgChanged || hasMarkupChanged || this.character != ' ')
                {
                    int requiredfg = this.fg;
                    int requiredbg = this.bg;

                    if (fgBuilder.Length < requiredfg)
                    {
                        int diff = requiredfg - fgBuilder.Length;
                        for (var i = 0; i < diff; i++) fgBuilder.Append(' ');
                    }

                    if (bgBuilder.Length < requiredbg)
                    {
                        int diff = requiredbg - bgBuilder.Length;
                        for (var i = 0; i < diff; i++) bgBuilder.Append(previous.isDefaultBG ? ' ' : '_');
                    }
                }

                // This is how we'll track our new length
                int bgLenold = bgBuilder.Length;
                int fgLenOld = fgBuilder.Length;

                // Skip rendering spaces when there's no markup to render
                if (this.character == ' ' && !hasMarkupChanged && !isFirst && !bgChanged)
                {
                    this.bgLen = 1;
                    this.fgLen = 1;
                    return;
                }

                if (bgChanged)
                {
                    if (!isLast)
                    {
                        bgBuilder.Insert(this.bg, ">_");
                        bgBuilder.Insert(this.bg, this.bgColor);
                        bgBuilder.Insert(this.bg, "<color=#");

                        if (!this.isDefaultBG)
                        {
                            bgBuilder.Insert(this.bg, "ff>");
                            bgBuilder.Insert(this.bg, this.bgColor);
                            bgBuilder.Insert(this.bg, "<mark=#");
                        }
                    }

                    if (!isFirst)
                        if (this.isDefaultBG)
                            bgBuilder.Insert(this.bg, "</mark>");
                }
                else
                {
                    if (this.isDefaultBG)
                        bgBuilder.Insert(this.bg, ' ');
                    else
                        bgBuilder.Insert(this.bg, '_');
                }

                // Insert the character itself.
                if (fuck)
                {
                    // We need to add an underscore here instead
                    // of a space, that way the background highlight shows correctly
                    fgBuilder.Insert(this.fg, $"<color=#00000000>_<color=#{this.fgColor}>");
                }
                else
                {
                    if (isLast && !this.isDefaultBG && this.character == ' ')
                    {
                        fgBuilder.Insert(this.fg, $"<color=#00000000>_<color=#{this.fgColor}>");
                    }
                    else
                    {
                        bool noparse = this.character is ('<' or '>');

                        if (noparse)
                            fgBuilder.Insert(this.fg, "</noparse>");

                        fgBuilder.Insert(this.fg, this.character);

                        if (noparse)
                            fgBuilder.Insert(this.fg, "<noparse>");
                    }
                }

                if (faintChanged)
                {
                    if (this.faint)
                        fgBuilder.Insert(this.fg, "<alpha=#7f>");
                    else
                        fgBuilder.Insert(this.fg, "<alpha=#ff>");
                }

                // Color
                if (fgChanged)
                {
                    fgBuilder.Insert(this.fg, '>');
                    fgBuilder.Insert(this.fg, this.fgColor);
                    fgBuilder.Insert(this.fg, "<color=#");
                }

                // Bold
                if (boldChanged)
                {
                    if (this.bold)
                        fgBuilder.Insert(this.fg, "<b>");
                    else if (!isFirst)
                        fgBuilder.Insert(this.fg, "</b>");
                }

                // Italic
                if (italicChanged)
                {
                    if (this.italic)
                        fgBuilder.Insert(this.fg, "<i>");
                    else if (!isFirst)
                        fgBuilder.Insert(this.fg, "</i>");
                }

                // Underline
                if (underlineChanged)
                {
                    if (this.underline)
                        fgBuilder.Insert(this.fg, "<u>");
                    else if (!isFirst)
                        fgBuilder.Insert(this.fg, "</u>");
                }


                // Struck
                if (struckChanged)
                {
                    if (this.struck)
                        fgBuilder.Insert(this.fg, "<s>");
                    else if (!isFirst)
                        fgBuilder.Insert(this.fg, "</s>");
                }

                // Update our lengths
                this.bgLen = Math.Max(bgBuilder.Length - bgLenold, 0);
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