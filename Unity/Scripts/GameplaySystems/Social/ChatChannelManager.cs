#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.WorldData.Data;
using Social;

namespace GameplaySystems.Social
{
	public class ChatChannelManager : IDisposable
	{
		private readonly ISocialService socialService;
		private readonly MessageManager messageManager;
		private readonly IWorldManager world;
		private readonly Dictionary<ObjectId, ChatChannel> channels = new Dictionary<ObjectId, ChatChannel>();

		public ChatChannelManager(ISocialService socialService, IWorldManager worldManager, MessageManager messageManager)
		{
			this.socialService = socialService;
			this.messageManager = messageManager;
			this.world = worldManager;
			
			this.world.Callbacks.AddCreateCallback<WorldChannelData>(OnCreateChannel);
			this.world.Callbacks.AddDeleteCallback<WorldChannelData>(OnDeleteChannel);
			this.world.Callbacks.AddModifyCallback<WorldChannelData>(OnModifyChannel);
			
		}

		/// <inheritdoc />
		public void Dispose()
		{
			this.world.Callbacks.RemoveCreateCallback<WorldChannelData>(OnCreateChannel);
			this.world.Callbacks.RemoveDeleteCallback<WorldChannelData>(OnDeleteChannel);
			this.world.Callbacks.RemoveModifyCallback<WorldChannelData>(OnModifyChannel);
		}

		public IEnumerable<IChatChannel> GetGuildChannels(ObjectId guildId)
		{
			return channels.Values.Where(x => x.ChannelType == MessageChannelType.Guild && x.GuildId == guildId);
		}
		
		private void OnModifyChannel(WorldChannelData subjectprevious, WorldChannelData subjectnew)
		{
			if (!channels.TryGetValue(subjectnew.InstanceId, out ChatChannel channel))
				return;
			
			channel.SetData(subjectnew);
		}

		private void OnDeleteChannel(WorldChannelData subject)
		{
			if (!channels.TryGetValue(subject.InstanceId, out ChatChannel channel))
				return;

			channel.Dispose();
			channels.Remove(subject.InstanceId);
		}

		private void OnCreateChannel(WorldChannelData subject)
		{
			if (!channels.TryGetValue(subject.InstanceId, out ChatChannel channel))
			{
				channel = new ChatChannel(messageManager, this.socialService);
				this.channels.Add(subject.InstanceId, channel);
			}
			
			channel.SetData(subject);
		}

		public IEnumerable<IChatChannel> GetDirectMessageChannels()
		{
			return this.channels.Values.Where(x => x.ChannelType == MessageChannelType.DirectMessage);
		}

		public IChatChannel? FindChannelById(ObjectId id, MessageChannelType type)
		{
			if (!channels.TryGetValue(id, out ChatChannel channel))
				return null;

			if (channel.ChannelType != type)
				return null;

			return channel;
		}

		public IChatChannel GetNarrativeChannel(ObjectId guildId, string channelId)
		{
			foreach (IChatChannel channel in GetGuildChannels(guildId))
			{
				if (channel.NarrativeId == channelId)
					return channel;
			}

			var newChannel = new WorldChannelData
			{
				InstanceId = world.GetNextObjectId(),
				GuildId = guildId,
				NarrativeId = channelId,
				Name = channelId
			};
			
			world.World.Channels.Add(newChannel);

			return channels[newChannel.InstanceId];
		}
	}
}