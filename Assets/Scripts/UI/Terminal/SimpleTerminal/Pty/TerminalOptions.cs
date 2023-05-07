namespace UI.Terminal.SimpleTerminal.Pty
{
    /// <summary>
    /// A class containing settings for a <see cref="PseudoTerminal"/> master/slave pair. 
    /// </summary>
    public class TerminalOptions
    {
#pragma warning disable CS1591
        public uint OFlag { set; get; }

        public uint LFlag { set; get; }

        public readonly byte[] C_cc = new byte[20];
    }
}