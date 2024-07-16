using AcidicGUI.Layout;
using AcidicGUI.Widgets;
using SociallyDistant.Core.UI.Common;
using SociallyDistant.Core.UI.VisualStyles;
using SociallyDistant.UI.Documents;

namespace SociallyDistant.UI.Tools.Chat;

public sealed class ChatMessageView : Widget
{
    private readonly FlexPanel                   root           = new();
    private readonly Avatar                      avatar         = new();
    private readonly DocumentAdapter<StackPanel> messageAdapter = new();
    private readonly Box                         messageBox     = new();

    public ChatMessageView()
    {
        avatar.VerticalAlignment = VerticalAlignment.Top;
        
        root.Direction = Direction.Horizontal;
        root.Spacing = 6;
        root.Padding = 6;
        Children.Add(root);
        root.ChildWidgets.Add(avatar);
        root.ChildWidgets.Add(messageBox);
        messageBox.Content = messageAdapter;
    }
    
    public void UpdateView(ChatMessageModel model)
    {
        avatar.Visibility = model.ShowAvatar
            ? Visibility.Visible
            : Visibility.Hidden;

        messageAdapter.ShowDocument(new[] { model.Document });

        if (model.UseBubbleStyle)
        {
            this.root.Reversed = model.IsFromPlayer;
            messageBox.SetCustomProperty(WidgetBackgrounds.ChatBubble);
            messageBox.SetCustomProperty(model.IsFromPlayer
                ? ChatBubbleColor.Player
                : ChatBubbleColor.Npc);
            messageBox.Margin = 12;
        }
        else
        {
            root.Reversed = false;
            messageBox.SetCustomProperty(WidgetBackgrounds.None);
            messageBox.SetCustomProperty(ChatBubbleColor.None);
            messageBox.Margin = 0;
        }
    }
}