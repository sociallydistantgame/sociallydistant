namespace SociallyDistant.Core.UI.Terminal;

public struct Selection
{
    public SelectionMode mode;
    public SelectionType type;
    public SelectionSnap snap;

    public IntPoint nb;
    public IntPoint ne;
    public IntPoint ob;
    public IntPoint oe;

    public bool alt;
}