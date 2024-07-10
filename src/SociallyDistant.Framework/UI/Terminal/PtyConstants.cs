namespace SociallyDistant.Core.UI.Terminal;

/// <summary>
/// termios constant values. Please see http://pubs.opengroup.org/onlinepubs/7908799/xsh/termios.h.html for more details.
/// </summary>
public static class PtyConstants
{
    public const uint VEOF   = 0;
    public const uint VEOL   = 1;
    public const uint VEOL2  = 2;
    public const uint VERASE = 3;
    public const uint VKILL  = 5;
    public const uint VINTR  = 8;
    public const uint VQUIT  = 9;
    public const uint VSUSP  = 10;
    public const uint NCCS   = 20;

    public const uint IGNBRK = 0x0001;
    public const uint BRKINT = 0x0002;
    public const uint ICRNL  = 0x01000;
    public const uint OPOST  = 0x0001;
    public const uint ONLCR  = 0x0002;
    public const uint OXTABS = 0x0004;
    public const uint ECHOKE = 0x0001;
    public const uint ECHOE  = 0x0002;
    public const uint ECHO   = 0x0008;
    public const uint ECHONL = 0x0010;

    public const uint ISIG   = 0x0080;
    public const uint ICANON = 0x0100;
    public const uint IEXTEN = 0x0400;
    public const uint CREAD  = 0x0800;
}