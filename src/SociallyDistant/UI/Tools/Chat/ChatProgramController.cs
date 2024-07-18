using System.Runtime.InteropServices;
using AcidicGUI.CustomProperties;
using AcidicGUI.Layout;
using AcidicGUI.ListAdapters;
using AcidicGUI.Widgets;
using Microsoft.Xna.Framework;
using SociallyDistant.Core;
using SociallyDistant.Core.Core.WorldData.Data;
using SociallyDistant.Core.Modules;
using SociallyDistant.Core.Programs;
using SociallyDistant.Core.Shell;
using SociallyDistant.Core.Social;
using SociallyDistant.Core.UI.Recycling;
using SociallyDistant.GameplaySystems.Chat;

namespace SociallyDistant.UI.Tools.Chat;

public sealed class ChatProgramController : ProgramController
{
    private readonly FlexPanel                        root              = new();
    private readonly FlexPanel                        channelsPanel     = new();
    private readonly FlexPanel                        guildsPanel       = new();
    private readonly FlexPanel                        conversationPanel = new();
    private readonly ChatHeader                       chatHeader        = new();
    private readonly ServerMembersList                membersPanel      = new();
    private readonly TypingIndicator                  typingIndicator   = new();
    private readonly ChatMessageListView              messagesList      = new();
    private readonly GuildButton                      homeButton        = new();
    private readonly GuildList                        guildList         = new();
    private readonly RecyclableWidgetList<ScrollView> channelsList      = new();
    private readonly ChatInputField                   messageInput      = new();
    private readonly ISocialService                   socialService;
    private readonly WorldManager                     worldManager;
    private readonly ConversationManager              conversationManager;
    private readonly List<ChatMessageModel>           messages     = new();
    private readonly List<IUserMessage>               userMessages = new List<IUserMessage>();
    private          IGuildList?                      playerGuilds;
    private          IGuild?                          currentGuild;
    private          IChatChannel?                    currentChannel = null;
    private          IDisposable?                     messageSendObserver;
    private          IDisposable?                     messageDeleteObserver;
    private          IDisposable?                     messageModifyObserver;
    private          IDisposable?                     guildJoinObserver;
    private          IDisposable?                     guildLeaveObserver;
    private          IDisposable?                     branchObserver;
    private          IDisposable?                     pendingChatBoxRequestsObserver;
    private          IDisposable?                     typingObserver;
    
    
    
    private ChatProgramController(ProgramContext context) : base(context)
    {
        homeButton.UseAvatar = false;
        homeButton.Icon = MaterialIcons.Home;
        
        root.Direction = Direction.Horizontal;
        root.Spacing = 12;
        root.Padding = 12;

        membersPanel.MinimumSize = new Point(160, 0);
        membersPanel.MaximumSize = new Point(160, 0);
        channelsPanel.MinimumSize = new Point(200, 0);
        channelsPanel.MaximumSize = new Point(200, 0);
        
        conversationPanel.GetCustomProperties<FlexPanelProperties>().Mode = FlexMode.Proportional;
        messagesList.GetCustomProperties<FlexPanelProperties>().Mode = FlexMode.Proportional;
        channelsList.GetCustomProperties<FlexPanelProperties>().Mode = FlexMode.Proportional;
        guildList.GetCustomProperties<FlexPanelProperties>().Mode = FlexMode.Proportional;
        
        this.socialService = Application.Instance.Context.SocialService;
        this.worldManager = WorldManager.Instance;
        this.conversationManager = ConversationManager.Instance!;

        context.Window.Content = root;
        root.ChildWidgets.Add(guildsPanel);
        root.ChildWidgets.Add(channelsPanel);
        root.ChildWidgets.Add(conversationPanel);
        root.ChildWidgets.Add(membersPanel);
        conversationPanel.ChildWidgets.Add(chatHeader);
        conversationPanel.ChildWidgets.Add(messagesList);
        conversationPanel.ChildWidgets.Add(typingIndicator);
        conversationPanel.ChildWidgets.Add(messageInput);
        guildsPanel.ChildWidgets.Add(homeButton);
        guildsPanel.ChildWidgets.Add(guildList);
        channelsPanel.ChildWidgets.Add(channelsList);

        messageInput.OnSubmit += OnMessageSubmit;
    }

    protected override void Main()
    {
        if (conversationManager == null)
        {
            CloseWindow();
            return;
        }
        
        pendingChatBoxRequestsObserver = conversationManager.ObservePendingChatBoxRequests(OnChatBoxControlRequested);
        playerGuilds = socialService.GetGuilds().ThatHaveMember(socialService.PlayerProfile);
        guildJoinObserver = playerGuilds.ObserveGuildAdded().Subscribe(OnGuildJoin);
        guildLeaveObserver = playerGuilds.ObserveGuildRemoved().Subscribe(OnGuildLeave);

        RefreshGuildList();

        IChatChannel? channel = socialService.GetDirectConversations(socialService.PlayerProfile).FirstOrDefault();
        if (channel != null)
            ShowChannel(channel);
        else
            ShowDirectMessagesList();
    }
    
    private void RefreshGuildList()
    {
        this.guildList.SetGuilds(playerGuilds.Select(x => new GuildItemModel() { Guild = x, IsSelected = x == currentGuild }).ToList());
    }
    
    public void ShowGuild(IGuild guild)
    {
        this.currentGuild = guild;

        this.membersPanel.Visibility = Visibility.Visible;
        this.membersPanel.SetItems(currentGuild.Members.Select(x => new ServerMember { Handle = x.Profile.ChatUsername, Name = x.Profile.ChatName, Avatar = x.Profile.Picture }).ToList());
			
        channelsList.SetWidgets(BuildGuildWidgets(guild));
    }
    
    private void ShowChannel(IChatChannel channel)
    {
        typingObserver?.Dispose();
        branchObserver?.Dispose();
			
        if (currentChannel != null)
        {
            messageSendObserver?.Dispose();
            messageDeleteObserver?.Dispose();
            messageModifyObserver?.Dispose();
        }

        currentChannel = channel;
			
        if (this.currentGuild != null && channel.ChannelType == MessageChannelType.Guild)
        {
            chatHeader.DisplayGuildHeader(channel.Name, channel.Description);
        }
        else
        {
            chatHeader.DisplayDirectMessage(channel.Name, channel.Description, null!);
        }

        userMessages.Clear();
        userMessages.AddRange(channel.Messages);
			
        this.messageSendObserver = currentChannel.SendObservable.Subscribe(OnMessageReceived);
        this.messageDeleteObserver = currentChannel.EditObservable.Subscribe(OnMessageEdit);
        this.messageDeleteObserver = currentChannel.DeleteObservable.Subscribe(OnMessageDelete);

        if (conversationManager != null)
        {
            branchObserver = conversationManager.ObserveBranchDefinitions(channel, OnNewBranchesAvailable);
        }

        typingObserver = channel.ObserveTypingUsers(OnTypingListChanged);

        if (channel.ChannelType == MessageChannelType.DirectMessage)
        {
            ShowDirectMessagesList();
        }
			
        UpdateMessageList();
    }
    
    public void ShowDirectMessagesList()
    {
        this.currentGuild = null;

        this.membersPanel.Visibility = Visibility.Collapsed;
			
        this.channelsList.SetWidgets(BuildDirectMessageList());
    }
    
    private void ConvertToUiMessage(IUserMessage userMessage, bool isNewMessage = false)
    {
        //notificationGroup.MarkNotificationAsRead(userMessage.Id);
			
        var newMessageShowsAvatar = true;
        if (messages.Count > 0)
        {
            ChatMessageModel lastModel = messages[^1];
            lastModel.IsNewMessage = false;
				
            if (lastModel.AuthorId == userMessage.Author.ProfileId && IsRecent(userMessage.Date, lastModel.Date))
                newMessageShowsAvatar = false;
        }

        foreach (DocumentElement documentElement in userMessage.GetDocumentData())
        {
            messages.Add(new ChatMessageModel()
            {
                Id = userMessage.Id,
                Date = userMessage.Date,
                AuthorId = userMessage.Author.ProfileId,
                UseBubbleStyle = !(currentGuild != null && currentChannel != null && currentChannel.ChannelType == MessageChannelType.Guild),
                IsFromPlayer = userMessage.Author.ProfileId == socialService.PlayerProfile.ProfileId,
                FormattedDateTime = FormatDate(userMessage.Date),
                Username = userMessage.Author.ChatUsername,
                DisplayName = userMessage.Author.ChatName,
                Document = documentElement,
                ShowAvatar = newMessageShowsAvatar,
                IsNewMessage = isNewMessage
            });
        }
    }
    
    private IList<RecyclableWidgetController> BuildDirectMessageList()
    {
        var builder = new WidgetBuilder();

        builder.Begin();
			
        builder.AddSection("Conversations", out SectionWidget section);

        var conversationCount = 0;

        foreach (IDirectConversation? channel in socialService.GetDirectConversations(socialService.PlayerProfile))
        {
            builder.AddWidget(new ListItemWidget<IChatChannel>
            {
                Image = BuildAvatarWidget(channel.GetIcon()),
                Title = $"<b>{channel.Name}</b>{Environment.NewLine}{channel.Description}",
                Data = channel,
                Callback = ShowChannel,
                Selected = this.currentChannel?.Id == channel.Id
            }, section);
				
            conversationCount++;
        }
			
        if (conversationCount == 0)
        {
            builder.AddWidget(new LabelWidget()
            {
                Text = "You do not have any active conversations. When you receive a direct message, it will appear here."
            }, section);
        }

        return builder.Build();
    }
    
    private IWidget? BuildAvatarWidget(ChannelIconData data)
    {
        // TODO
        if (data.UseUnicodeIcon)
            return null;

        return new AvatarWidget()
        {
            Size = AvatarSize.Small,
            AvatarColor = data.AvatarColor,
            AvatarTexture = data.UserAvatar
        };
    }
    
    private IList<RecyclableWidgetController> BuildGuildWidgets(IGuild guild)
    {
        var builder = new WidgetBuilder();

        builder.Begin();

        builder.AddSection("Channels", out SectionWidget? channelSection);

        int memberCount = guild.Members.Count();
        var channelCount = 0;

        foreach (IChatChannel channel in guild.Channels)
        {
            builder.AddWidget(new ListItemWidget<IChatChannel>
            {
                Title = channel.Name,
                Data = channel,
                Callback = ShowChannel
            }, channelSection);
				
            channelCount++;
        }

        if (channelCount == 0)
        {
            builder.AddWidget(new LabelWidget
            {
                Text = "This server has no channels."
            }, channelSection);
        }

        /* builder.AddWidget(new GuildHeaderWidget
        {
            Name = guild.Name,
            MemberCount = memberCount,
            ChannelCount = channelCount
        }); */
			
			
        return builder.Build();
    }
    
    
    
    private void UpdateMessageList()
    {
        this.messages.Clear();

        foreach (IUserMessage message in userMessages)
            ConvertToUiMessage(message);
			
        this.messagesList.SetMessages(this.messages);
    }
    
    private void OnTypingListChanged(IEnumerable<IProfile> typers)
    {
        this.typingIndicator.UpdateIndicator(typers);
    }
    
    private void OnNewBranchesAvailable(BranchDefinitionList list)
    {
        this.messageInput.UpdateBranchList(list);
    }
    
    private void OnMessageReceived(IUserMessage message)
    {
        userMessages.Add(message);
			
        ConvertToUiMessage(message, true);
        this.messagesList.SetMessages(this.messages);
    }
    
    private void OnMessageEdit(IUserMessage message)
    {
        UpdateMessageList();
    }
    
    private void OnMessageDelete(IUserMessage message)
    {
        userMessages.Remove(message);
        UpdateMessageList();
    }
    
    private async void OnChatBoxControlRequested(ChatBoxRequest request)
    {
        if (this.currentChannel == null)
            return;

        if (request.ChannelId != this.currentChannel.Id)
            return;

        await request.GiveControlAndWaitForRelease(this.messageInput);
    }
    
    private void OnMessageSubmit(string text)
    {
        if (this.messageInput.PickSelectedBranchIfAny())
        {
            this.messageInput.SetTextWithoutNotify(string.Empty);
            return;
        }
			
        if (string.IsNullOrWhiteSpace(text))
            return;
			
        if (currentChannel == null)
            return;

        var message = new WorldMessageData
        {
            InstanceId = worldManager.GetNextObjectId(),
            ChannelId = currentChannel.Id,
            Author = socialService.PlayerProfile.ProfileId,
            Date = DateTime.UtcNow,
            DocumentElements = new List<DocumentElement>()
            {
                new DocumentElement()
                {
                    ElementType = DocumentElementType.Text,
                    Data = text
                }
            }
        };
			
        worldManager.World.Messages.Add(message);
			
        this.messageInput.SetTextWithoutNotify(string.Empty);
    }
    
    private void OnGuildLeave(IGuild guild)
    {
        if (guild == currentGuild)
            ShowDirectMessagesList();
			
        RefreshGuildList();
    }
    
    private void OnGuildJoin(IGuild guild)
    {
        ShowGuild(guild);
        RefreshGuildList();
    }
    
    private bool IsRecent(DateTime newDate, DateTime oldDate)
    {
        TimeSpan timespan = newDate - oldDate;

        return timespan.TotalMinutes <= 30;
    }
    
    private string FormatDate(DateTime date)
    {
        return date.ToLongDateString();
    }
}