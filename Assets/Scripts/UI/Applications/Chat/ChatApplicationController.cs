#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using AcidicGui.Widgets;
using Core;
using Core.WorldData.Data;
using GamePlatform;
using GameplaySystems.Chat;
using GameplaySystems.Social;
using Shell.Common;
using Social;
using TMPro;
using UI.PlayerUI;
using UI.Widgets;
using UI.Widgets.Settings;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions;
using UniRx;

namespace UI.Applications.Chat
{
	public class ChatApplicationController : MonoBehaviour
	{
		[Header("UI")]
		[SerializeField]
		private ChatHeader chatHeader = null!;

		[SerializeField]
		private ServerMembersController serverMembers = null!;

		[SerializeField]
		private TypingIndicator typingIndicator = null!;
		
		[SerializeField]
		private ChatConversationController conversationController = null!;

		[SerializeField]
		private GuildListView guildList = null!;

		[SerializeField]
		private Toggle homeButton = null!;

		[SerializeField]
		private WidgetList channelList = null!;

		[SerializeField]
		private TMP_InputField messageInputField = null!;

		[SerializeField]
		private ConversationBranchList branchList = null!;
		
		private readonly List<ChatMessageModel> messages = new List<ChatMessageModel>();
		private readonly List<IUserMessage> userMessages = new List<IUserMessage>();
		private ConversationManager? conversationManager;
		private IGuildList? playerGuilds;
		private IGuild? currentGuild;
		private IChatChannel? currentChannel = null;
		private UiManager uiManager = null!;
		private IDisposable? messageSendObserver;
		private IDisposable? messageDeleteObserver;
		private IDisposable? messageModifyObserver;
		private IDisposable? guildJoinObserver;
		private IDisposable? guildLeaveObserver;
		private IDisposable? branchObserver;
		private IDisposable? pendingChatBoxRequestsObserver;
		private IDisposable? typingObserver;
		private IWorldManager worldManager;
		private ISocialService socialService;
		private INotificationGroup notificationGroup;
		
		private void Awake()
		{
			worldManager = GameManager.Instance.WorldManager;
			socialService = GameManager.Instance.SocialService;
			conversationManager = ConversationManager.Instance;

			notificationGroup = GameManager.Instance.NotificationManager.GetNotificationGroup(NotificationGroups.Chat);
			
			this.AssertAllFieldsAreSerialized(typeof(ChatApplicationController));
			this.MustGetComponentInParent(out uiManager);
		}

		private void OnEnable()
		{
			if (conversationManager != null)
			{
				pendingChatBoxRequestsObserver = conversationManager.ObservePendingChatBoxRequests(OnChatBoxControlRequested);
			}

			if (currentChannel != null)
			{
				userMessages.Clear();

				foreach (IUserMessage? message in currentChannel.Messages)
				{
					userMessages.Add(message);
					notificationGroup.MarkNotificationAsRead(message.Id);
				}

				UpdateMessageList();

				this.messageSendObserver = currentChannel.SendObservable.Subscribe(OnMessageReceived);
				this.messageDeleteObserver = currentChannel.EditObservable.Subscribe(OnMessageEdit);
				this.messageDeleteObserver = currentChannel.DeleteObservable.Subscribe(OnMessageDelete);
			}
		}

		private void OnDisable()
		{
			this.messageSendObserver?.Dispose();
            this.messageSendObserver = null;
			this.messageDeleteObserver?.Dispose();
            this.messageDeleteObserver = null;
			this.messageDeleteObserver?.Dispose();
            this.messageDeleteObserver = null;
			this.pendingChatBoxRequestsObserver?.Dispose();
		}

		private void Start()
		{
			playerGuilds = socialService.GetGuilds().ThatHaveMember(socialService.PlayerProfile);
			guildJoinObserver = playerGuilds.ObserveGuildAdded().Subscribe(OnGuildJoin);
			guildLeaveObserver = playerGuilds.ObserveGuildRemoved().Subscribe(OnGuildLeave);
			
			
			homeButton.onValueChanged.AddListener(OnHomeButtonToggle);
			guildList.GuildSelected += ShowGuild;
			messageInputField.onSubmit.AddListener(OnMessageSubmit);

			RefreshGuildList();

			IChatChannel? channel = socialService.GetDirectConversations(socialService.PlayerProfile).FirstOrDefault();
			if (channel != null)
				ShowChannel(channel);
			else
				ShowDirectMessagesList();
		}

		private async void OnChatBoxControlRequested(ChatBoxRequest request)
		{
			if (this.currentChannel == null)
				return;

			if (request.ChannelId != this.currentChannel.Id)
				return;

			uiManager.Autopilot = true;

			await request.GiveControlAndWaitForRelease(this.messageInputField);
			
			uiManager.Autopilot = false;
		}

		private void OnDestroy()
		{
			messageSendObserver?.Dispose();
			messageDeleteObserver?.Dispose();
			messageModifyObserver?.Dispose();
			guildJoinObserver?.Dispose();
			guildLeaveObserver?.Dispose();
			branchObserver?.Dispose();
			pendingChatBoxRequestsObserver?.Dispose();
			typingObserver?.Dispose();
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

		private void OnTypingListChanged(IEnumerable<IProfile> typers)
		{
			this.typingIndicator.UpdateIndicator(typers);
		}
		
		private void UpdateMessageList()
		{
			this.messages.Clear();

			foreach (IUserMessage message in userMessages)
				ConvertToUiMessage(message);
			
			this.conversationController.SetMessageList(this.messages);
		}
		
		public void ShowGuild(IGuild guild)
		{
			this.currentGuild = guild;

			this.serverMembers.IsVisible = true;
			this.serverMembers.UpdateMembers(currentGuild.Members.Select(x => new ServerMember
			{
				Username = x.Profile.ChatUsername,
				DisplayName = x.Profile.ChatName
			}).ToList());
			
			channelList.SetItems(BuildGuildWidgets(guild));
		}

		public void ShowDirectMessagesList()
		{
			this.currentGuild = null;
			homeButton.SetIsOnWithoutNotify(true);

			this.serverMembers.IsVisible = false;
			
			this.channelList.SetItems(BuilDDirectMessageList());
		}
		
		private void OnHomeButtonToggle(bool isOn)
		{
			if (isOn)
				ShowDirectMessagesList();
		}

		private void RefreshGuildList()
		{
			this.guildList.SetItems(playerGuilds.Select(x => new GuildItemModel()
			{
				Guild = x,
				ToggleGroup = this.homeButton.group,
				IsSelected = x == currentGuild
			}).ToList());
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
		
		private IList<IWidget> BuildGuildWidgets(IGuild guild)
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

			builder.AddWidget(new GuildHeaderWidget
			{
				Name = guild.Name,
				MemberCount = memberCount,
				ChannelCount = channelCount
			});
			
			
			return builder.Build();
		}

		private void OnNewBranchesAvailable(BranchDefinitionList list)
		{
			this.branchList.UpdateList(list);
		}

		private bool IsRecent(DateTime newDate, DateTime oldDate)
		{
			TimeSpan timespan = newDate - oldDate;

			return timespan.TotalMinutes <= 30;
		}
		
		private void ConvertToUiMessage(IUserMessage userMessage, bool isNewMessage = false)
		{
			notificationGroup.MarkNotificationAsRead(userMessage.Id);
			
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

		private string FormatDate(DateTime date)
		{
			return date.ToLongDateString();
		}

		private void OnMessageReceived(IUserMessage message)
		{
			userMessages.Add(message);
			
			ConvertToUiMessage(message, true);
			conversationController.SetMessageList(this.messages);
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
		
		private void OnMessageSubmit(string text)
		{
			if (this.branchList.PickSelectedIfAny())
			{
				this.messageInputField.SetTextWithoutNotify(string.Empty);
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
			
			this.messageInputField.SetTextWithoutNotify(string.Empty);

			messageInputField.ActivateInputField();
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

		private IList<IWidget> BuilDDirectMessageList()
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
	}
}