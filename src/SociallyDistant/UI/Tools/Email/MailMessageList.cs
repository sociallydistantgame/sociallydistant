using AcidicGUI.ListAdapters;
using AcidicGUI.Widgets;
using SociallyDistant.Core.Social;

namespace SociallyDistant.UI.Tools.Email;

public sealed class MailMessageList : ListAdapter<ScrollView, MailMessageViewHolder>
{
    private readonly DataHelper<IMailMessage> messages;
    private          IMailMessage?            activeMessage;

    public event Action<IMailMessage>? MessageClicked;
    
    public IMailMessage? SelectedMessage
    {
        get => activeMessage;
        set => activeMessage = value;
    }

    public MailMessageList()
    {
        messages = new DataHelper<IMailMessage>(this);
    }
    
    public void SetItems(IEnumerable<IMailMessage> source)
    {
        messages.SetItems(source);
    }
    
    protected override MailMessageViewHolder CreateViewHolder(int itemIndex, Box rootWidget)
    {
        return new MailMessageViewHolder(itemIndex, rootWidget);
    }

    protected override void UpdateView(MailMessageViewHolder viewHolder)
    {
        IMailMessage message = messages[viewHolder.ItemIndex];
        viewHolder.IsActive = message == activeMessage;
        viewHolder.UpdateView(message);
        viewHolder.Callback = OnMessageClicked;
    }

    private void OnMessageClicked(IMailMessage message)
    {
        MessageClicked?.Invoke(message);
    }
}