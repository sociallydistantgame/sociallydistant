#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.WorldData;
using Core.WorldData.Data;
using Social;
using UniRx;

namespace GameplaySystems.Social
{
	public class MessageManager : IDisposable
	{
		private readonly Dictionary<ObjectId, UserMessage> messages = new Dictionary<ObjectId, UserMessage>();
		private readonly IWorldManager worldManager;
		private readonly SocialService socialService;
		private readonly Subject<IUserMessage> messageCreateSubject = new Subject<IUserMessage>();
		private readonly Subject<IUserMessage> messageModifySubject = new Subject<IUserMessage>();
		private readonly Subject<IUserMessage> messageDeleteSubject = new Subject<IUserMessage>();

		public IObservable<IUserMessage> MessageCreateObservable => messageCreateSubject;
		public IObservable<IUserMessage> MessageDeleteObservable => messageDeleteSubject;
		public IObservable<IUserMessage> MessageModifyObservable => messageModifySubject;

		internal MessageManager(IWorldManager worldManager, SocialService socialService)
		{
			this.worldManager = worldManager;
			this.socialService = socialService;
			
			this.worldManager.Callbacks.AddCreateCallback<WorldMessageData>(OnCreateMessage);
			this.worldManager.Callbacks.AddModifyCallback<WorldMessageData>(OnModifyMessage);
			this.worldManager.Callbacks.AddDeleteCallback<WorldMessageData>(OnDeleteMessage);
			
		}

		/// <inheritdoc />
		public void Dispose()
		{
			messageCreateSubject.Dispose();
			messageModifySubject.Dispose();
			messageDeleteSubject.Dispose();
			
			this.worldManager.Callbacks.RemoveCreateCallback<WorldMessageData>(OnCreateMessage);
			this.worldManager.Callbacks.RemoveModifyCallback<WorldMessageData>(OnModifyMessage);
			this.worldManager.Callbacks.RemoveDeleteCallback<WorldMessageData>(OnDeleteMessage);
		}

		public IEnumerable<IUserMessage> GetMessagesInChannel(ObjectId channelId)
		{
			return messages.Values.Where(x => x.ChannelId == channelId)
				.OrderBy(x => x.Date);
		}
		
		private void OnModifyMessage(WorldMessageData subjectprevious, WorldMessageData subjectnew)
		{
			if (!messages.TryGetValue(subjectnew.InstanceId, out UserMessage message))
				return;
            
			message.SetData(subjectnew);
			messageModifySubject.OnNext(message);
		}

		private void OnDeleteMessage(WorldMessageData subject)
		{
			if (!messages.TryGetValue(subject.InstanceId, out UserMessage message))
				return;

			messageDeleteSubject.OnNext(message);
			messages.Remove(subject.InstanceId);
		}

		private void OnCreateMessage(WorldMessageData subject)
		{
			if (!this.messages.TryGetValue(subject.InstanceId, out UserMessage message))
			{
				message = new UserMessage(this.socialService);
				this.messages.Add(subject.InstanceId, message);
			}
			
			message.SetData(subject);
			messageCreateSubject.OnNext(message);
		}
	}
}