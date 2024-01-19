#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using AcidicGui.Widgets;
using Core;
using Core.WorldData.Data;
using GameplaySystems.Social;
using Social;
using TMPro;
using UI.Widgets;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions;
using UniRx;

namespace UI.Applications.Chat
{
	public class ChatApplicationController : MonoBehaviour
	{
		[Header("Dependencies")]
		[SerializeField]
		private WorldManagerHolder worldManager = null!;
		
		[SerializeField]
		private SocialServiceHolder socialService = null!;
		
		[Header("UI")]
		[SerializeField]
		private ChatHeader chatHeader = null!;

		[SerializeField]
		private ServerMembersController serverMembers = null!;

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
		
		private readonly List<ChatMessageModel> messages = new List<ChatMessageModel>();
		private readonly List<IUserMessage> userMessages = new List<IUserMessage>();
		private IDisposable? messageSendObserver;
		private IDisposable? messageDeleteObserver;
		private IDisposable? messageModifyObserver;
		private IGuildList? playerGuilds;
		private IDisposable? guildJoinObserver;
		private IDisposable? guildLeaveObserver;
		private IGuild? currentGuild;
		private IChatChannel? currentChannel = null;
		
		
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(ChatApplicationController));
		}

		private void Start()
		{
			if (socialService.Value == null)
				return;
			
			playerGuilds = socialService.Value.GetGuilds().ThatHaveMember(socialService.Value.PlayerProfile);
			guildJoinObserver = playerGuilds.ObserveGuildAdded().Subscribe(OnGuildJoin);
			guildLeaveObserver = playerGuilds.ObserveGuildRemoved().Subscribe(OnGuildLeave);
			
			
			homeButton.onValueChanged.AddListener(OnHomeButtonToggle);
			guildList.GuildSelected += ShowGuild;
			messageInputField.onSubmit.AddListener(OnMessageSubmit);

			RefreshGuildList();
			ShowDirectMessagesList();
		}

		private void OnDestroy()
		{
			playerGuilds?.Dispose();
		}

		private void ShowChannel(IChatChannel channel)
		{
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
            
			UpdateMessageList();
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

			ListWidget? list = null;

			foreach (IChatChannel channel in guild.Channels)
			{
				if (list == null)
				{
					list = new ListWidget
					{
						AllowSelectNone = false
					};

					builder.AddWidget(list, channelSection);
				}

				builder.AddWidget(new ListItemWidget<IChatChannel>
				{
					List = list,
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

		private bool IsRecent(DateTime newDate, DateTime oldDate)
		{
			TimeSpan timespan = newDate - oldDate;

			return timespan.TotalMinutes <= 5;
		}
		
		private void ConvertToUiMessage(IUserMessage userMessage)
		{
			if (messages.Count > 0)
			{
				ChatMessageModel lastModel = messages[^1];
				
				// We can merge documents from the new message into the last message if
				// the last message was sent by the same author and was sent within the last 5 minutes
				// of the new message.
				if (lastModel.AuthorId == userMessage.Author.ProfileId && IsRecent(userMessage.Date, lastModel.Date))
				{
					DocumentElement[] oldDocs = lastModel.DocumentData;
					DocumentElement[] newDocs = userMessage.GetDocumentData();
                    
					Array.Resize(ref oldDocs, oldDocs.Length + newDocs.Length);
					Array.Copy(newDocs, 0, oldDocs, oldDocs.Length - newDocs.Length, newDocs.Length);

					lastModel.DocumentData = oldDocs;
					
					return;
				}
			}
			
			messages.Add(new ChatMessageModel()
			{
				Date = userMessage.Date,
				AuthorId = userMessage.Author.ProfileId,
				UseBubbleStyle = !(currentGuild != null && currentChannel != null && currentChannel.ChannelType == MessageChannelType.Guild),
				IsFromPlayer = socialService.Value != null && userMessage.Author.ProfileId == socialService.Value.PlayerProfile.ProfileId,
				FormattedDateTime = FormatDate(userMessage.Date),
				Username = userMessage.Author.ChatUsername,
				DisplayName = userMessage.Author.ChatName,
				DocumentData = userMessage.GetDocumentData()
			});
		}

		private string FormatDate(DateTime date)
		{
			return date.ToLongDateString();
		}

		private void OnMessageReceived(IUserMessage message)
		{
			userMessages.Add(message);
			UpdateMessageList();
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
			if (string.IsNullOrWhiteSpace(text))
				return;

			if (worldManager.Value == null)
				return;

			if (socialService.Value == null)
				return;

			if (currentChannel == null)
				return;

			var message = new WorldMessageData
			{
				InstanceId = worldManager.Value.GetNextObjectId(),
				ChannelId = currentChannel.Id,
				Author = socialService.Value.PlayerProfile.ProfileId,
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
			
			worldManager.Value.World.Messages.Add(message);
			
			this.messageInputField.SetTextWithoutNotify(string.Empty);

			messageInputField.ActivateInputField();
		}

		private IList<IWidget> BuilDDirectMessageList()
		{
			var builder = new WidgetBuilder();

			builder.Begin();
			
			builder.AddSection("Conversations", out SectionWidget section);

			var conversationCount = 0;
			ListWidget? list = null;

			foreach (IDirectConversation? channel in socialService.Value.GetDirectConversations(socialService.Value.PlayerProfile))
			{
				if (list == null)
				{
					list = new ListWidget
					{
						AllowSelectNone = false
					};

					builder.AddWidget(list, section);
				}

				builder.AddWidget(new ListItemWidget<IChatChannel>
				{
					List = list,
					Title = $"<b>{channel.Name}</b>{Environment.NewLine}{channel.Description}",
					Data = channel,
					Callback = ShowChannel
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