namespace SociallyDistant.UI;

public sealed class DockModel
{
    private readonly List<DockGroup> groups = new();

    public DockGroup DefineGroup()
    {
        var group = new DockGroup(this);
        groups.Add(group);
        return group;
    }
}