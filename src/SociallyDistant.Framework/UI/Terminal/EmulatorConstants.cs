namespace SociallyDistant.Core.UI.Terminal;

public static class EmulatorConstants
{
    public const uint UTF_INVALID      = 0xFFFD;
    public const int  UTF_SIZ_PLUS_ONE = UTF_SIZ + 1;
    public const int  UTF_SIZ          = 4;
    public const uint ESC_BUF_SIZ      = 128 * UTF_SIZ;
    public const uint ESC_ARG_SIZ      = 16;
    public const uint STR_BUF_SIZ      = ESC_BUF_SIZ;
    public const uint STR_ARG_SIZ      = ESC_ARG_SIZ;
}