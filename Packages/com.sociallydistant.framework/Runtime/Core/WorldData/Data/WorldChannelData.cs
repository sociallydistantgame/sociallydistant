using System.Collections.Generic;
using Core.Serialization;

namespace Core.WorldData.Data
{
	public struct WorldChannelData :
		IWorldData,
		IDataWithId,
		INarrativeObject
	{
		private ObjectId id;
		private string name;
		private string description;
		private byte channelType;
		private ObjectId guildId;
		private IReadOnlyList<ObjectId> typingUsers;
		private string? narrativeId;
		
		/// <inheritdoc />
		public void Serialize(IWorldSerializer serializer)
		{
			// We DO NOT serialize typing users, we only need to track them as part of CRUD events in the chat system.
			typingUsers = new List<ObjectId>();
			
			SerializationUtility.SerializeAtRevision(ref id, serializer, WorldRevision.ChatAndSocialMedia, default);
			SerializationUtility.SerializeAtRevision(ref name, serializer, WorldRevision.ChatAndSocialMedia, default);
			SerializationUtility.SerializeAtRevision(ref description, serializer, WorldRevision.ChatAndSocialMedia, default);
			SerializationUtility.SerializeAtRevision(ref channelType, serializer, WorldRevision.ChatAndSocialMedia, default);
			SerializationUtility.SerializeAtRevision(ref guildId, serializer, WorldRevision.ChatAndSocialMedia, default);
			SerializationUtility.SerializeAtRevision(ref narrativeId, serializer, WorldRevision.NarrativeChannels, default);
			
		}

		public IReadOnlyList<ObjectId> TypingUsers
		{
			get => typingUsers;
			set => typingUsers = value;
		}
		
		/// <inheritdoc />
		public ObjectId InstanceId
		{
			get => id;
			set => id = value;
		}

		public string Name
		{
			get => name;
			set => name = value;
		}

		public string Description
		{
			get => description;
			set => description = value;
		}

		public MessageChannelType ChannelType
		{
			get => (MessageChannelType) channelType;
			set => channelType = (byte) value;
		}

		public ObjectId GuildId
		{
			get => guildId;
			set => guildId = value;
		}

		/// <inheritdoc />
		public string? NarrativeId
		{
			get => narrativeId;
			set => narrativeId = value;
		}
	}
}