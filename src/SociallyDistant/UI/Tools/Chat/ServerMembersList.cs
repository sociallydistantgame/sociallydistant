using AcidicGUI.ListAdapters;
using AcidicGUI.Widgets;

namespace SociallyDistant.UI.Tools.Chat;

public sealed class ServerMembersList : ListAdapter<ScrollView, ServerMemberViewHolder>
{
    private readonly DataHelper<ServerMember> items;

    public ServerMembersList()
    {
        items = new DataHelper<ServerMember>(this);
    }

    public void SetItems(IEnumerable<ServerMember> source)
    {
        items.SetItems(source);
    }

    protected override ServerMemberViewHolder CreateViewHolder(int itemIndex, Box rootWidget)
    {
        return new ServerMemberViewHolder(itemIndex, rootWidget);
    }

    protected override void UpdateView(ServerMemberViewHolder viewHolder)
    {
        viewHolder.UpdateView(items[viewHolder.ItemIndex]);
    }
}