using AcidicGUI.ListAdapters;
using AcidicGUI.Widgets;
using SociallyDistant.Core.Social;

namespace SociallyDistant.UI.Tools.Email;

public sealed class MailMessageViewHolder : ViewHolder
{
    private readonly MailMessageListItem view = new();

    public Action<IMailMessage>? Callback
    {
        get => view.Callback;
        set => view.Callback = value;
    }
    
    public bool IsActive
    {
        get => view.IsActive;
        set => view.IsActive = value;
    }
    
    public MailMessageViewHolder(int itemIndex, Box root) : base(itemIndex, root)
    {
        root.Content = view;
    }
    
    public void UpdateView(IMailMessage message)
    {
        this.view.UpdateView(message);
    }
}