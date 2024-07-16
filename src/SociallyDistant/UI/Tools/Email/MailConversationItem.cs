using AcidicGUI.Widgets;
using SociallyDistant.Core.Core;
using SociallyDistant.Core.Core.WorldData.Data;
using SociallyDistant.Core.Social;
using SociallyDistant.UI.Common;
using SociallyDistant.UI.Documents;

namespace SociallyDistant.UI.Tools.Email;

public sealed class MailConversationItem : Widget
{
    private readonly InfoBox                     root          = new();
    private readonly StackPanel                  messageArea   = new();
    private readonly TextWidget                  metadata      = new();
    private readonly DocumentAdapter<StackPanel> document      = new();
    private readonly List<DocumentElement>       finalDocument = new();

    public MailConversationItem()
    {
        messageArea.Spacing = 12;
        root.UseOpaqueBlock = true;
        metadata.UseMarkup = true;
        metadata.WordWrapping = true;

        document.Container.Spacing = 6;
        
        Children.Add(root);
        root.Content = messageArea;
        messageArea.ChildWidgets.Add(metadata);
        messageArea.ChildWidgets.Add(document);
    }
    
    public void UpdateView(IMailMessage message)
    {
        finalDocument.Clear();
        
        metadata.Text = SociallyDistantUtility.CreateFormattedDataMarkup(new Dictionary<string, string>() { { "From", message.From.ChatName }, { "To", message.To.ChatName }, });

        finalDocument.AddRange(message.Body);

        if ((message.MessageType & MailTypeFlags.Briefing) != 0)
        {
            finalDocument.Add(new DocumentElement { ElementType = DocumentElementType.Mission, Data = message.NarrativeId });
        }

        this.document.ShowDocument(finalDocument);
    }
}