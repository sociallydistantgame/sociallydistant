#nullable enable
using System;
using System.Collections.Generic;
using Core;
using Core.WorldData.Data;
using Social;
using UniRx;

namespace GameplaySystems.Social
{
	public class ChatChannel : 
		IChatChannel,
		IDisposable
	{
		private readonly MessageManager messageManager;
		private readonly Subject<IUserMessage> sendSubject = new Subject<IUserMessage>();
		private readonly Subject<IUserMessage> editSubject = new Subject<IUserMessage>();
		private readonly Subject<IUserMessage> deleteSubject = new Subject<IUserMessage>();
		private readonly IDisposable createObserver;
		private readonly IDisposable deleteObserver;
		private readonly IDisposable modifyObserver;
		
		private ObjectId id;

		/// <inheritdoc />
		public ObjectId Id => id;

		/// <inheritdoc />
		public string Name { get; private set; }

		/// <inheritdoc />
		public string Description { get; private set; }

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

		public ChatChannel(MessageManager messageManager)
		{
			this.messageManager = messageManager;

			this.createObserver = this.messageManager.MessageCreateObservable.Subscribe(OnMessageCreate);
			this.deleteObserver = this.messageManager.MessageDeleteObservable.Subscribe(OnMessageDelete);
			this.modifyObserver = this.messageManager.MessageModifyObservable.Subscribe(OnMessageEdit);
		}
		
		public void SetData(WorldChannelData data)
		{
			this.id = data.InstanceId;
			
			Name = data.Name;
			Description = data.Description;
			ChannelType = data.ChannelType;
			GuildId = data.GuildId;
		}

		/// <inheritdoc />
		public void Dispose()
		{
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