namespace SociallyDistant.Core.UI.Terminal;

[Flags]
public enum SelectionSnap : byte
{
    NONE      = 0,
    SNAP_WORD = 1,
    SNAP_LINE = 2
}