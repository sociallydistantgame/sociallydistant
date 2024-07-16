using AcidicGUI.Layout;
using AcidicGUI.TextRendering;
using AcidicGUI.Widgets;
using SociallyDistant.Core.Shell;
using SociallyDistant.Core.Social;
using SociallyDistant.Core.UI.Common;

namespace SociallyDistant.UI.Tools.Email;

public sealed class MailMessageListItem : Widget
{
    private readonly ListItem            root      = new();
    private readonly StackPanel          stack     = new();
    private readonly CompositeIconWidget icon      = new();
    private readonly StackPanel          textStack = new();
    private readonly TextWidget          subject   = new();
    private readonly TextWidget          author    = new();
    private          IMailMessage?       message;

    public bool IsActive
    {
        get => root.IsActive;
        set => root.IsActive = value;
    }
    
    public Action<IMailMessage>? Callback { get; set; }
    
    public MailMessageListItem()
    {
        stack.Direction = Direction.Horizontal;
        stack.Spacing = 6;
        icon.VerticalAlignment = VerticalAlignment.Middle;
        textStack.VerticalAlignment = VerticalAlignment.Middle;
        subject.FontWeight = FontWeight.SemiBold;
        icon.IconSize = 24;
        icon.Icon = MaterialIcons.MailOutline;
        
        Children.Add(root);
        root.Content = stack;
        stack.ChildWidgets.Add(icon);
        stack.ChildWidgets.Add(textStack);
        textStack.ChildWidgets.Add(subject);
        textStack.ChildWidgets.Add(author);

        root.ClickCallback = OnClick;
    }

    private void OnClick()
    {
        if (message == null)
            return;
        
        Callback?.Invoke(message);
    }

    public void UpdateView(IMailMessage message)
    {
        this.message = message;
        subject.Text = message.Subject;
        author.Text = message.From.ChatName;
    }
}