using AcidicGUI.ListAdapters;
using AcidicGUI.Widgets;

namespace SociallyDistant.UI.Tools.Chat;

public sealed class ChatMessageViewHolder : ViewHolder
{
    private readonly ChatMessageView view = new();
    
    public ChatMessageViewHolder(int itemIndex, Box root) : base(itemIndex, root)
    {
        root.Content = view;
    }

    public void UpdateView(ChatMessageModel model)
    {
        view.UpdateView(model);
    }
}