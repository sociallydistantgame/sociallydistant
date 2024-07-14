using AcidicGUI.ListAdapters;
using AcidicGUI.Widgets;

namespace SociallyDistant.UI.Shell;

public sealed class DockGroupAdapter : ListAdapter<StackPanel, DockIconViewHolder>
{
    private readonly DataHelper<DockGroup.IconDefinition> icons;
    
    public DockGroupAdapter()
    {
        icons = new DataHelper<DockGroup.IconDefinition>(this);
        Container.Spacing = 3;
    }

    public void SetItems(IEnumerable<DockGroup.IconDefinition> newIcons)
    {
        this.icons.SetItems(newIcons);
    }
    
    protected override DockIconViewHolder CreateViewHolder(int itemIndex, Box rootWidget)
    {
        return new DockIconViewHolder(itemIndex, rootWidget);
    }

    protected override void UpdateView(DockIconViewHolder viewHolder)
    {
        viewHolder.UpdateView(icons[viewHolder.ItemIndex]);
    }
}