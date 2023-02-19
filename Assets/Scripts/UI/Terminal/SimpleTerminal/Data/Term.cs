using System;

namespace UI.Terminal.SimpleTerminal.Data
{
    public class Term
    {
        public int row;
        public int col;
        public Line[] line = Array.Empty<Line>();
        public Line[] alt = Array.Empty<Line>();
        public int[] dirty = Array.Empty<int>();
        public TCursor c;
        public int ocx;
        public int ocy;
        public int top;
        public int bot;
        public TermMode mode;
        public EscapeState esc;
        public Charset[] trantbl = new Charset[4];
        public int charset;
        public int icharset;
        public int[] tabs = Array.Empty<int>();
        public uint lastc;
        public int histi;
        public Line[] hist = Array.Empty<Line>();
        public int scr;
        public int histf;
        public int[] wrapcwidth = new int[2];
    }
}