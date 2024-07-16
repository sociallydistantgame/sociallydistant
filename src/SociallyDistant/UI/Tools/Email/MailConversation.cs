using AcidicGUI.ListAdapters;
using AcidicGUI.Widgets;
using SociallyDistant.Core.Social;
using SociallyDistant.GameplaySystems.Mail;

namespace SociallyDistant.UI.Tools.Email;

public sealed class MailConversation : ListAdapter<ScrollView, MailConversationViewHolder>
{
    private readonly DataHelper<IMailMessage> messages;

    public MailConversation()
    {
        messages = new DataHelper<IMailMessage>(this);
    }

    public void ViewMessage(MailManager mailManager, IMailMessage? focus)
    {
        if (focus == null || focus.Thread == null)
        {
            messages.SetItems(Enumerable.Empty<IMailMessage>());
            return;
        }

        messages.SetItems(focus.Thread.GetMessagesInThread());
    }
    
    protected override MailConversationViewHolder CreateViewHolder(int itemIndex, Box rootWidget)
    {
        return new MailConversationViewHolder(itemIndex, rootWidget);
    }

    protected override void UpdateView(MailConversationViewHolder viewHolder)
    {
        IMailMessage message = messages[viewHolder.ItemIndex];
        viewHolder.UpdateView(message);
    }
}