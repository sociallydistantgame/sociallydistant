using AcidicGUI.Layout;
using AcidicGUI.ListAdapters;
using AcidicGUI.Widgets;

namespace SociallyDistant.UI.Tools.Chat;

public sealed class ChatMessageListView : ListAdapter<ScrollView, ChatMessageViewHolder>
{
    private readonly DataHelper<ChatMessageModel> messages;

    public ChatMessageListView()
    {
        messages = new DataHelper<ChatMessageModel>(this);
        Container.VerticalAlignment = VerticalAlignment.Bottom;
        Container.Spacing = 6;
    }

    public void SetMessages(IEnumerable<ChatMessageModel> source)
    {
        messages.SetItems(source);
        Container.ScrollToEnd();
    }
    
    protected override ChatMessageViewHolder CreateViewHolder(int itemIndex, Box rootWidget)
    {
        return new ChatMessageViewHolder(itemIndex, rootWidget);
    }

    protected override void UpdateView(ChatMessageViewHolder viewHolder)
    {
        viewHolder.UpdateView(messages[viewHolder.ItemIndex]);
    }
}