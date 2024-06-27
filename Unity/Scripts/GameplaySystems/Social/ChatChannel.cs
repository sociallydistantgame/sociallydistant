#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.WorldData.Data;
using Shell;
using Social;
using UniRx;

namespace GameplaySystems.Social
{
	public class ChatChannel : 
		IChatChannel,
		IDisposable
	{
		private readonly Subject<IEnumerable<IProfile>> typersSubject = new();
		private readonly ISocialService socialService;
		private readonly List<IProfile> typers = new(0);
		private readonly MessageManager messageManager;
		private readonly Subject<IUserMessage> sendSubject = new Subject<IUserMessage>();
		private readonly Subject<IUserMessage> editSubject = new Subject<IUserMessage>();
		private readonly Subject<IUserMessage> deleteSubject = new Subject<IUserMessage>();
		private readonly IDisposable createObserver;
		private readonly IDisposable deleteObserver;
		private readonly IDisposable modifyObserver;
		
		private ObjectId id;

		/// <inheritdoc />
		public string? NarrativeId { get; private set; }

		/// <inheritdoc />
		public ObjectId Id => id;

		/// <inheritdoc />
		public string Name { get; private set; }

		/// <inheritdoc />
		public string Description { get; private set; }

		/// <inheritdoc />
		public IEnumerable<IProfile> TypingUsers => typers;

		/// <inheritdoc />
		public IObservable<IUserMessage> SendObservable => sendSubject;

		/// <inheritdoc />
		public IObservable<IUserMessage> EditObservable => editSubject;

		/// <inheritdoc />
		public IObservable<IUserMessage> DeleteObservable => deleteSubject;

		/// <inheritdoc />
		public MessageChannelType ChannelType { get; private set; }

		/// <inheritdoc />
		public ObjectId? GuildId { get; private set; }

		/// <inheritdoc />
		public IEnumerable<IUserMessage> Messages => messageManager.GetMessagesInChannel(this.id);

		/// <inheritdoc />
		public IDisposable ObserveTypingUsers(Action<IEnumerable<IProfile>> callback)
		{
			callback?.Invoke(typers);
			return typersSubject.Subscribe(callback);
		}

		/// <inheritdoc />
		public ChannelIconData GetIcon()
		{
			return new ChannelIconData
			{
				UseUnicodeIcon = true,
				UnicodeIcon = MaterialIcons.ChatBubbleOutline
			};
		}

		public ChatChannel(MessageManager messageManager, ISocialService socialService)
		{
			this.messageManager = messageManager;
			this.socialService = socialService;

			this.createObserver = this.messageManager.MessageCreateObservable.Subscribe(OnMessageCreate);
			this.deleteObserver = this.messageManager.MessageDeleteObservable.Subscribe(OnMessageDelete);
			this.modifyObserver = this.messageManager.MessageModifyObservable.Subscribe(OnMessageEdit);
		}
		
		public void SetData(WorldChannelData data)
		{
			NarrativeId = data.NarrativeId;
			this.id = data.InstanceId;
			
			Name = data.Name;
			Description = data.Description;
			ChannelType = data.ChannelType;
			GuildId = data.GuildId;

			this.typers.Clear();
			
			if (data.TypingUsers != null)
				this.typers.AddRange(data.TypingUsers.Select(socialService.GetProfileById));

			this.typersSubject.OnNext(this.typers);
		}

		/// <inheritdoc />
		public void Dispose()
		{
			typersSubject.Dispose();
			sendSubject?.Dispose();
			createObserver?.Dispose();
			
			editSubject?.Dispose();
			modifyObserver?.Dispose();

			deleteSubject?.Dispose();
			deleteObserver?.Dispose();
		}

		private void OnMessageCreate(IUserMessage message)
		{
			if (message.ChannelId != this.id)
				return;
			
			sendSubject.OnNext(message);
		}

		private void OnMessageDelete(IUserMessage message)
		{
			if (message.ChannelId != this.id)
				return;
			
			deleteSubject.OnNext(message);
		}
		
		private void OnMessageEdit(IUserMessage message)
		{
			if (message.ChannelId != this.id)
				return;
			
			editSubject.OnNext(message);
		}
	}
}