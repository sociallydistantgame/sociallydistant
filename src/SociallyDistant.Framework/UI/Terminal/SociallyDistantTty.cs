namespace SociallyDistant.Core.UI.Terminal;

public class SociallyDistantTty : IPseudoTerminal
{
    private readonly Stream ttyStream;

    public SociallyDistantTty(Stream stream)
    {
        this.ttyStream = stream;
    }

    /// <inheritdoc />
    public int Read(byte[] buffer, int offset, int length)
    {
        return ttyStream.Read(buffer, offset, length);
    }

    /// <inheritdoc />
    public void Write(byte[] buffer, int offset, int count)
    {
        ttyStream.Write(buffer, offset, count);
    }
}