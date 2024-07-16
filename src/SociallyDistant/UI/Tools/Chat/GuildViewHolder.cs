using AcidicGUI.ListAdapters;
using AcidicGUI.Widgets;
using SociallyDistant.Core.Shell;

namespace SociallyDistant.UI.Tools.Chat;

public sealed class GuildViewHolder : ViewHolder
{
    private readonly GuildButton view = new();
    
    public GuildViewHolder(int itemIndex, Box root) : base(itemIndex, root)
    {
        root.Content = view;
    }

    public void UpdateView(GuildItemModel model)
    {
        view.UseAvatar = model.GuildIcon != null;
        view.Avatar = model.GuildIcon;
        view.Icon = MaterialIcons.Group;
    }
}