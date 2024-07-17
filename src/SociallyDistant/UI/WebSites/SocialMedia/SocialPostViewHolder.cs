using AcidicGUI.ListAdapters;
using AcidicGUI.Widgets;

namespace SociallyDistant.UI.WebSites.SocialMedia;

public sealed class SocialPostViewHolder : ViewHolder
{
    private readonly SocialPostView view = new();
    
    public SocialPostViewHolder(int itemIndex, Box root) : base(itemIndex, root)
    {
        root.Content = view;
    }

    public void UpdateView(SocialPostModel model)
    {
        this.view.UpdateView(model);
    }
}