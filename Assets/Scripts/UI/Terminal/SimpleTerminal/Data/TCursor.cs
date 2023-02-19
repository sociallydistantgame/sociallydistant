namespace UI.Terminal.SimpleTerminal.Data
{
    public struct TCursor
    {
        public Glyph attr;
        public int x;
        public int y;
        public CursorState state;
    }
}