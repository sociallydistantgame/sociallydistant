using System.Runtime.InteropServices;
using AcidicGUI.CustomProperties;
using AcidicGUI.Layout;
using AcidicGUI.TextRendering;
using AcidicGUI.Widgets;
using Microsoft.Xna.Framework;
using SociallyDistant.Core.Modules;
using SociallyDistant.Core.Programs;
using SociallyDistant.Core.Shell;
using SociallyDistant.Core.Social;
using SociallyDistant.Core.UI.Common;
using SociallyDistant.Core.UI.VisualStyles;
using SociallyDistant.GameplaySystems.Mail;

namespace SociallyDistant.UI.Tools.Email;

public sealed class EmailProgramController : ProgramController
{
    private readonly FlexPanel        root           = new();
    private readonly FlexPanel        sidebar        = new();
    private readonly FlexPanel        mainArea       = new();
    private readonly StackPanel       myUserArea     = new();
    private readonly Avatar           myUserAvatar   = new();
    private readonly StackPanel       myUserTextArea = new();
    private readonly TextWidget       myName         = new();
    private readonly TextWidget       myEmailAddress = new();
    private readonly TextWidget       subject        = new();
    private readonly ISocialService   socialService;
    private readonly MailManager?     mailManager;
    private readonly MailMessageList  messageList  = new();
    private readonly MailConversation conversation = new();
    private          InboxMode        inboxMode;
    private          IMailMessage?    currentMessage;
    
    
    private EmailProgramController(ProgramContext context) : base(context)
    {
        socialService = Application.Instance.Context.SocialService;
        mailManager = MailManager.Instance;
        
        context.Window.Title = "Email";

        root.Padding = 12;
        root.Spacing = 12;
        
        root.Direction = Direction.Horizontal;
        myUserArea.Direction = Direction.Horizontal;
        myUserArea.Spacing = 3;
        myUserAvatar.VerticalAlignment = VerticalAlignment.Middle;
        myUserTextArea.VerticalAlignment = VerticalAlignment.Middle;

        mainArea.GetCustomProperties<FlexPanelProperties>().Mode = FlexMode.Proportional;
        messageList.GetCustomProperties<FlexPanelProperties>().Mode = FlexMode.Proportional;
        conversation.GetCustomProperties<FlexPanelProperties>().Mode = FlexMode.Proportional;
        

        sidebar.MinimumSize = new Point(280, 0);
        sidebar.MaximumSize = new Point(280, 0);
        
        subject.FontSize = 24;
        subject.SetCustomProperty(WidgetForegrounds.Common);
        subject.SetCustomProperty(CommonColor.Blue);
        subject.FontWeight = FontWeight.SemiBold;
        subject.WordWrapping = true;

        myName.FontWeight = FontWeight.SemiBold;
        myUserAvatar.AvatarSize = 48;

        context.Window.Content = root;

        root.ChildWidgets.Add(sidebar);
        root.ChildWidgets.Add(mainArea);
        sidebar.ChildWidgets.Add(myUserArea);
        myUserArea.ChildWidgets.Add(myUserAvatar);
        myUserArea.ChildWidgets.Add(myUserTextArea);
        myUserTextArea.ChildWidgets.Add(myName);
        myUserTextArea.ChildWidgets.Add(myEmailAddress);
        mainArea.ChildWidgets.Add(subject);
        sidebar.ChildWidgets.Add(messageList);
        mainArea.ChildWidgets.Add(conversation);
        
        messageList.MessageClicked += OnMessageClicked;
    }

    private void OnMessageClicked(IMailMessage message)
    {
        this.currentMessage = message;
        UpdateScreen();
    }

    protected override void Main()
    {
        if (mailManager == null)
        {
            CloseWindow();
            return;
        }

        UpdateScreen();
    }
    
    private void UpdateScreen()
    {
        if (mailManager == null)
            return;
			
        IProfile user = socialService.PlayerProfile;

        IEnumerable<IMailMessage> messagesToList = this.inboxMode switch
        {
            InboxMode.Inbox => this.mailManager.GetMessagesForUser(user),
            InboxMode.Outbox => mailManager.GetMessagesFromUser(user),
            InboxMode.CompletedMissions => Enumerable.Empty<IMailMessage>()
        };
			
        myName.Text = user.ChatName;
        myEmailAddress.Text = user.ChatUsername;
        messageList.SelectedMessage = currentMessage;
        messageList.SetItems(messagesToList);
        

        subject.Text = currentMessage?.Subject ?? string.Empty;
        conversation.ViewMessage(mailManager, this.currentMessage);
    }
    
    private enum InboxMode
    {
        Inbox,
        Outbox,
        CompletedMissions
    }
}