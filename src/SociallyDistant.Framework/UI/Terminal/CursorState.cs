namespace SociallyDistant.Core.UI.Terminal;

[Flags]
public enum CursorState : byte
{
    CURSOR_DEFAULT  = 0,
    CURSOR_WRAPNEXT = 1,
    CURSOR_ORIGIN   = 2
}