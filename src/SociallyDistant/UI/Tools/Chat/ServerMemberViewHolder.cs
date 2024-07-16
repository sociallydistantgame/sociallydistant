using AcidicGUI.ListAdapters;
using AcidicGUI.Widgets;

namespace SociallyDistant.UI.Tools.Chat;

public sealed class ServerMemberViewHolder : ViewHolder
{
    private readonly ServerMemberItem view = new();

    public void UpdateView(ServerMember model)
    {
        this.view.UpdateView(model);
    }

    public ServerMemberViewHolder(int itemIndex, Box root) : base(itemIndex, root)
    {
        root.Content = view;
    }
}