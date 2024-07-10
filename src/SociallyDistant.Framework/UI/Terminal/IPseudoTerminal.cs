namespace SociallyDistant.Core.UI.Terminal;

public interface IPseudoTerminal
{
    int Read(byte[] buffer, int offset, int length);
    void Write(byte[] buffer, int offset, int count);
}