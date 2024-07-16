using AcidicGUI.ListAdapters;
using AcidicGUI.Widgets;
using SociallyDistant.Core.Social;

namespace SociallyDistant.UI.Tools.Email;

public sealed class MailConversationViewHolder : ViewHolder
{
    private readonly MailConversationItem view = new();
    
    public MailConversationViewHolder(int itemIndex, Box root) : base(itemIndex, root)
    {
        root.Content = view;
    }

    public void UpdateView(IMailMessage message)
    {
        this.view.UpdateView(message);
    }
}