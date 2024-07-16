using AcidicGUI.ListAdapters;
using AcidicGUI.Widgets;

namespace SociallyDistant.UI.Tools.Chat;

public sealed class GuildList : ListAdapter<ScrollView, GuildViewHolder>
{
    private readonly DataHelper<GuildItemModel> guilds;

    public GuildList()
    {
        guilds = new DataHelper<GuildItemModel>(this);
    }

    public void SetGuilds(IEnumerable<GuildItemModel> source)
    {
        guilds.SetItems(source);
    }

    protected override GuildViewHolder CreateViewHolder(int itemIndex, Box rootWidget)
    {
        return new GuildViewHolder(itemIndex, rootWidget);
    }

    protected override void UpdateView(GuildViewHolder viewHolder)
    {
        viewHolder.UpdateView(guilds[viewHolder.ItemIndex]);
    }
}