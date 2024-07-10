namespace SociallyDistant.Core.UI.Terminal;

/// <summary>
/// A class containing settings for a <see cref="PseudoTerminal"/> master/slave pair. 
/// </summary>
public class TerminalOptions
{
    public uint OFlag { set; get; }

    public uint LFlag { set; get; }

    public readonly byte[] C_cc = new byte[20];
}