using System.Reflection.Metadata;
using AcidicGUI.ListAdapters;
using AcidicGUI.Widgets;
using SociallyDistant.UI.Common;

namespace SociallyDistant.UI.Shell;

public sealed class DockIconViewHolder : ViewHolder
{
    private readonly DockIconView view = new();
    
    public DockIconViewHolder(int itemIndex, Box root) : base(itemIndex, root)
    {
        root.Content = view;
    }

    public void UpdateView(DockGroup.IconDefinition model)
    {
        this.view.UpdateView(model);
    }
}