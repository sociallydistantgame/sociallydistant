using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using OS.Devices;
using UI.Terminal.SimpleTerminal.Data;
using UI.Terminal.SimpleTerminal.Pty;
using UnityEngine;
using Utility;
using static UI.Terminal.SimpleTerminal.Data.EmulatorConstants;

namespace UI.Terminal.SimpleTerminal
{
    public class SimpleTerminalSession : ITextConsoleWithPhysicalDisplay
    {
        private LineEditorState lineEditorState;
        private readonly SimpleTerminal term;
        private readonly PseudoTerminal pty;
        private readonly LineWrapper lineWrapper = new LineWrapper();
        private readonly byte[] ttybuf = new byte[1024];
        private int ttybuflen;
        private readonly Queue<string> lineQueue = new Queue<string>();
        private STREscape strescseq = new STREscape();
        private CSIEscape csiescseq = new CSIEscape();
        private readonly List<string> completions = new List<string>();
        private readonly ConcurrentQueue<ConsoleInputData> pendingKeys = new ConcurrentQueue<ConsoleInputData>();
        private readonly RepeatableCancellationToken token;
        
        private int prevLineCount;
        private StringBuilder line = new StringBuilder();
        private int cursor;
        private IAutoCompleteSource? autoCompleteSource;
        private bool completionsAreDirty;
        private int selectedCompletion;
        private int completionInsertionPoint;

        public IAutoCompleteSource? AutoCompleteSource
        {
            get => autoCompleteSource;
            set
            {
                autoCompleteSource = value;
                completionsAreDirty = true;
            }
        }

        public bool SuppressInput
        {
            get => term.SuppressInput;
            set => term.SuppressInput = value;
        }

        /// <inheritdoc />
        public ConsoleInputData? ReadInput()
        {
            this.PtyRead();

            if (!pendingKeys.TryDequeue(out ConsoleInputData data))
                return null;
                
            return data;
        }

        public SimpleTerminalSession(SimpleTerminal term, PseudoTerminal pty, RepeatableCancellationToken token)
        {
            this.term = term;
            this.pty = pty;
            this.token = token;
        }

        private void HandleLeftArrow()
        {
            this.pendingKeys.Enqueue(new ConsoleInputData(KeyCode.LeftArrow));
        }

        private void HandleUpArrow()
        {
            this.pendingKeys.Enqueue(new ConsoleInputData(KeyCode.UpArrow));
        }

        private void HandleDownArrow()
        {
            this.pendingKeys.Enqueue(new ConsoleInputData(KeyCode.DownArrow));
        }

        private void HandleRightArrow()
        {
            this.pendingKeys.Enqueue(new ConsoleInputData(KeyCode.RightArrow));
        }

        private void HandleHome()
        {
            this.pendingKeys.Enqueue(new ConsoleInputData(KeyCode.Home));
        }

        private void HandleEnd()
        {
            this.pendingKeys.Enqueue(new ConsoleInputData(KeyCode.End));
        }

        public void WriteText(string text)
        {
            /*if (Environment.OSVersion.Platform != PlatformID.Win32NT)
            {
                // Workaround: Strip CRLF, replace with LF, and replace LF with CRLF because I'm too
                // damn lazy to fix the renderer
                text = text.Replace("\r\n", "\n").Replace("\n", Environment.NewLine);
            }*/

            token.ThrowIfCancellationRequested();
            
            if (string.IsNullOrEmpty(text))
                return;

            byte[] bytes = Encoding.UTF8.GetBytes(text);
            this.pty.Write(bytes, 0, bytes.Length);
        }

        /// <inheritdoc />
        public string WindowTitle
        {
            get => this.term.WindowTitle;
            set => this.term.WindowTitle = value;
        }

        /// <inheritdoc />
        public bool IsInteractive => true;

        public void ClearScreen()
        {
            // ANSI: clear screen
            this.WriteText("\x1b[2J");

            // ANSI move cursor to the top-left
            this.WriteText("\x1b[1;1f");
        }

        private int PtyRead()
        {
            token.ThrowIfCancellationRequested();
            
            var ret = 0;
            var written = 0;

            ret = this.pty.Read(this.ttybuf, this.ttybuflen, this.ttybuf.Length - this.ttybuflen);

            if (ret > 0)
            {
                this.ttybuflen += ret;
                written = this.TerminalWrite(this.ttybuf, this.ttybuflen, false);
                this.ttybuflen -= written;

                if (this.ttybuflen > 0)
                    Array.Copy(this.ttybuf, 0, this.ttybuf, written, this.ttybuflen);
            }

            return ret;
        }

        public bool TryDequeueSubmittedInput(out string input)
        {
            // This will do the actual reading from the pty device.
            // It also handles line-editing.
            int ret = this.PtyRead();

            // Re-render the line edit state
            if (ret > 0 || this.lineEditorState.wasPositionReset)
            {
                this.lineEditorState.wasPositionReset = false;
                this.SetupLineEditor();
            }

            // This will dequeue any submitted lines.
            bool result = this.lineQueue.TryDequeue(out input);

            if (result)
            {
                this.lineEditorState.isEditing = false;
                this.WriteText(input);
                this.WriteText(Environment.NewLine);
            }

            return result;
        }

        private int TerminalWrite(byte[] buf, int buflen, bool showControl)
        {
            var charsize = 0;
            var u = 0u;
            var n = 0;

            for (n = 0; n < buflen; n += charsize)
            {
                charsize = TermUtf8.utf8decode(buf, n, out u, buflen - n);
                if (charsize == 0)
                    break;


                this.PutChar(u);
            }

            return n;
        }

        private void PutChar(uint u)
        {
            unsafe
            {
                byte* c = stackalloc byte[(int)UTF_SIZ];
                var control = false;
                var len = 0;
                var width = 0;

                control = SimpleTerminal.IsControl(u);

                if (u < 127)
                {
                    c[0] = (byte)u;
                    width = len = 1;
                }
                else
                {
                    len = TermUtf8.utf8encode(u, c);

                    if (!control && (width = SimpleTerminal.Wcwidth(u)) == -1)
                        width = 1;
                }

                if ((this.lineEditorState.esc & EscapeState.ESC_STR) != 0)
                {
                    if (u == '\a' || u == 030 || u == 032 || u == 033 ||
                        SimpleTerminal.Iscontrolc1(u))
                    {
                        this.lineEditorState.esc &= ~(EscapeState.ESC_START | EscapeState.ESC_STR);
                        this.lineEditorState.esc |= EscapeState.ESC_STR_END;
                        goto CheckControlCode;
                    }

                    if (this.strescseq.len + len >= this.strescseq.siz)
                    {
                        /*
                         * Here is a bug in terminals. If the user never sends
                         * some code to stop the str or esc command, then st
                         * will stop responding. But this is better than
                         * silently failing with unknown characters. At least
                         * then users will report back.
                         *
                         * In the case users ever get fixed, here is the code:
                         */
                        /*
                         * term.esc = 0;
                         * strhandle();
                         */
                        if (this.strescseq.siz > (int.MaxValue - UTF_SIZ) / 2)
                            return;
                        this.strescseq.siz *= 2;
                        this.strescseq.buf =
                            SimpleTerminal.XRealloc(ref this.strescseq.buf, this.strescseq.siz);
                    }

                    var ptr = new IntPtr(c);
                    Marshal.Copy(ptr, this.strescseq.buf, this.strescseq.len, len);
                    this.strescseq.len += len;
                    return;
                }

                CheckControlCode:

                if (control)
                {
                    this.ControlCode(u);
                    return;
                }
                else if ((this.lineEditorState.esc & EscapeState.ESC_START) != 0)
                {
                    if ((this.lineEditorState.esc & EscapeState.ESC_CSI) != 0)
                    {
                        this.csiescseq.buf[this.csiescseq.len++] = (byte)u;
                        if (SimpleTerminal.Between(u, 0x40, 0x7E) ||
                            this.csiescseq.len >= this.csiescseq.buf.Length - 1)
                        {
                            this.lineEditorState.esc = 0;
                            this.CsiParse();
                            this.CsiHandle();
                        }

                        return;
                    }
                    else
                    {
                        if (!this.EscHandle(u))
                            return;
                    }

                    this.lineEditorState.esc = 0;
                    return;
                }

                this.pendingKeys.Enqueue(new ConsoleInputData((char) u));
            }
        }

        private void ControlCode(uint ascii)
        {
            switch (ascii)
            {
                case '\t': /* HT */
                    this.pendingKeys.Enqueue(new ConsoleInputData(KeyCode.Tab));
                    return;
                case '\b': /* BS */
                    this.pendingKeys.Enqueue(new ConsoleInputData(KeyCode.Backspace));
                    return;
                case '\r': /* CR */
                    this.pendingKeys.Enqueue(new ConsoleInputData(KeyCode.Return));
                    return;
                case 0x1b: /* ESC */
                    this.CsiReset();
                    this.lineEditorState.esc &=
                        ~(EscapeState.ESC_CSI | EscapeState.ESC_ALTCHARSET | EscapeState.ESC_TEST);
                    this.lineEditorState.esc |= EscapeState.ESC_START;
                    return;
                case 0x18: /* CAN */
                    this.CsiReset();
                    break;
                case 0x7f: /* DEL */
                    this.pendingKeys.Enqueue(new ConsoleInputData(KeyCode.Delete));
                    break;
                case 0x90: /* DCS -- Device Control String */
                case 0x9d: /* OSC -- Operating System Command */
                case 0x9e: /* PM -- Privacy Message */
                case 0x9f: /* APC -- Application Program Command */
                    this.StrSequence(ascii);
                    return;
            }

            /* only CAN, SUB, \a and C1 chars interrupt a sequence */
            this.lineEditorState.esc &= ~(EscapeState.ESC_STR_END | EscapeState.ESC_STR);
        }

        private void StrSequence(uint ascii)
        {
        }

        private void StrHandle()
        {
        }

        private void CsiReset()
        {
            this.csiescseq = new CSIEscape();
        }

        private void CsiParse()
        {
            unsafe
            {
                fixed (byte* p = this.csiescseq.buf)
                {
                    byte* np = null;
                    long v = 0;

                    byte* p2 = p;

                    this.csiescseq.narg = 0;
                    if (*p2 == '?')
                    {
                        this.csiescseq.priv = 1;
                        p2++;
                    }

                    this.csiescseq.buf[this.csiescseq.len] = 0;
                    while (p2 < p + this.csiescseq.len)
                    {
                        np = null;
                        byte** nnp = &np;
                        v = GottaGoFast.strtol(p2, ref nnp, 10);
                        if (np == p)
                            v = 0;
                        if (v == long.MaxValue || v == long.MinValue)
                            v = -1;
                        this.csiescseq.arg[this.csiescseq.narg++] = (int)v;
                        p2 = np;
                        if (*p2 != ';' || this.csiescseq.narg == ESC_ARG_SIZ)
                            break;
                        p2++;
                    }

                    this.csiescseq.mode[0] = *p2++;
                    this.csiescseq.mode[1] = (byte)(p2 < p + this.csiescseq.len ? *p2 : 0);
                }
            }
        }

        private void CsiHandle()
        {
            unsafe
            {
                byte* buf = stackalloc byte[40];

                switch ((char)this.csiescseq.mode[0])
                {
                    default:
                        unknown:
                        Debug.LogError(
                            "Hey. Restitch yourself, terminal. You sent a bad escape sequence to the line editor.");
                        break;
                    case '@': /* ICH -- Insert <n> blank char */
                        goto unknown;
                    case 'A': /* CUU -- Cursor <n> Up */
                        this.HandleUpArrow();
                        break;
                    case 'B': /* CUD -- Cursor <n> Down */
                    case 'e': /* VPR --Cursor <n> Down */
                        this.HandleDownArrow();
                        break;
                    case 'i': /* MC -- Media Copy */
                    case 'c': /* DA -- Device Attributes */
                    case 'b': /* REP -- if last char is printable print it <n> more times */
                        goto unknown;
                    case 'C': /* CUF -- Cursor <n> Forward */
                    case 'a': /* HPR -- Cursor <n> Forward */
                        this.HandleRightArrow();
                        break;
                    case 'D': /* CUB -- Cursor <n> Backward */
                        this.HandleLeftArrow();
                        break;
                    case 'E': /* CNL -- Cursor <n> Down and first col */
                    case 'F': /* CPL -- Cursor <n> Up and first col */
                    case 'g': /* TBC -- Tabulation clear */
                    case 'G': /* CHA -- Move to <col> */
                    case '`': /* HPA */
                    case 'H': /* CUP -- Move to <row> <col> */
                    case 'f': /* HVP */
                    case 'I': /* CHT -- Cursor Forward Tabulation <n> tab stops */
                    case 'J': /* ED -- Clear screen */
                    case 'K': /* EL -- Clear line */
                        goto unknown;
                    case 'S': /* SU -- Scroll <n> line up */
                        break;
                    case 'T': /* SD -- Scroll <n> line down */
                        break;
                    case 'L': /* IL -- Insert <n> blank lines */
                    case 'l': /* RM -- Reset Mode */
                    case 'M': /* DL -- Delete <n> lines */
                    case 'X': /* ECH -- Erase <n> char */
                    case 'P': /* DCH -- Delete <n> char */
                    case 'Z': /* CBT -- Cursor Backward Tabulation <n> tab stops */
                    case 'd': /* VPA -- Move to <row> */
                    case 'h': /* SM -- Set terminal mode */
                    case 'm': /* SGR -- Terminal attribute (color) */
                    case 'n': /* DSR – Device Status Report (cursor position) */
                    case 'r': /* DECSTBM -- Set Scrolling Region */
                    case 's': /* DECSC -- Save cursor position (ANSI.SYS) */
                    case 'u': /* DECRC -- Restore cursor position (ANSI.SYS) */
                    case ' ':
                        goto unknown;
                    case '~': // Socially Distant keystroke events. See https://man.sociallydistantgame.com/index.php/Terminal_Escape_Sequences#Keystroke_events
                        if (csiescseq.narg < 2)
                            return;
                        
                        DecodeAndSendKeystroke(csiescseq.arg[0], csiescseq.arg[1]);
                        break;
                }
            }
        }

        private void DecodeAndSendKeystroke(int keyCode, int modifiersMask)
        {
            bool control = (modifiersMask & 1) != 0;
            bool alt = (modifiersMask & 2) != 0;
            bool shift = (modifiersMask & 4) != 0;

            var unityKeyCode = (KeyCode) keyCode;

            KeyModifiers modifiers = default;

            if (control)
                modifiers |= KeyModifiers.Control;

            if (alt)
                modifiers |= KeyModifiers.Alt;

            if (shift)
                modifiers |= KeyModifiers.Shift;
            
            pendingKeys.Enqueue(new ConsoleInputData(unityKeyCode, modifiers));
        }
        
        private bool EscHandle(uint ascii)
        {
            switch (ascii)
            {
                case '[':
                    this.lineEditorState.esc |= EscapeState.ESC_CSI;
                    return false;
                case '#':
                    this.lineEditorState.esc |= EscapeState.ESC_TEST;
                    return false;
                case '%':
                    this.lineEditorState.esc |= EscapeState.ESC_UTF8;
                    return false;
                case 'P': /* DCS -- Device Control String */
                case '_': /* APC -- Application Program Command */
                case '^': /* PM -- Privacy Message */
                case ']': /* OSC -- Operating System Command */
                case 'k': /* old title set compatibility */
                    this.StrSequence(ascii);
                    return false;
                case 'n': /* LS2 -- Locking shift 2 */
                case 'o': /* LS3 -- Locking shift 3 */
                case '(': /* GZD4 -- set primary charset G0 */
                case ')': /* G1D4 -- set secondary charset G1 */
                case '*': /* G2D4 -- set tertiary charset G2 */
                case '+': /* G3D4 -- set quaternary charset G3 */
                case 'D': /* IND -- Linefeed */
                    break;
                case 'E': /* NEL -- Next line */
                case 'H': /* HTS -- Horizontal tab stop */
                case 'M': /* RI -- Reverse index */
                case 'Z': /* DECID -- Identify Terminal */
                case 'c': /* RIS -- Reset to initial state */
                case '=': /* DECPAM -- Application keypad */
                case '>': /* DECPNM -- Normal keypad */
                case '7': /* DECSC -- Save Cursor */
                case '8': /* DECRC -- Restore Cursor */
                    break;
                case '\\': /* ST -- String Terminator */
                    if ((this.lineEditorState.esc & EscapeState.ESC_STR_END) != 0) this.StrHandle();
                    break;
                default:
                    break;
            }

            return true;
        }
        
        private void SetupLineEditor()
        {
            if (!this.lineEditorState.isEditing)
            {
                this.lineEditorState.isEditing = true;
                this.lineEditorState.firstLineColumn = this.term.CursorLeft;
                this.lineEditorState.firstLineRow = this.term.CursorTop;
                this.prevLineCount = 0;
            }
        }

        private struct LineEditorState
        {
            public bool isEditing;
            public int firstLineRow;
            public int firstLineColumn;
            public EscapeState esc;
            public bool wasPositionReset;
        }

        /// <inheritdoc />
        public int CursorLeft => term.CursorLeft;

        /// <inheritdoc />
        public int CursorTop => term.CursorTop;

        /// <inheritdoc />
        public int Width => term.Columns;

        /// <inheritdoc />
        public int Height => term.Rows;
    }
}