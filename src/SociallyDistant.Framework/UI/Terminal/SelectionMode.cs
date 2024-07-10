namespace SociallyDistant.Core.UI.Terminal;

[Flags]
public enum SelectionMode : byte
{
    SEL_IDLE  = 0,
    SEL_EMPTY = 1,
    SEL_READY = 2
}