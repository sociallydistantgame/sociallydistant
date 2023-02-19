namespace UI.Terminal.SimpleTerminal.Pty
{
    /// <summary>
    /// A class containing settings for a <see cref="PseudoTerminal"/> master/slave pair. 
    /// </summary>
    public class TerminalOptions
    {
        public int CursorLeft { get; set; }
        public int CursorTop { get; set; }
        public int RowCount { get; set; }
        public int ColumnCount { get; set; }

#pragma warning disable CS1591
        public uint OFlag { set; get; }

        public uint LFlag { set; get; }

        public readonly byte[] C_cc = new byte[20];
    }
}