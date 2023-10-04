using Core.Serialization;

namespace Core.WorldData.Data
{
	public struct WorldChannelData :
		IWorldData,
		IDataWithId
	{
		private ObjectId id;
		private string name;
		private string description;
		private byte channelType;
		private ObjectId guildId;
		
		/// <inheritdoc />
		public void Serialize(IWorldSerializer serializer)
		{
			SerializationUtility.SerializeAtRevision(ref id, serializer, WorldRevision.ChatAndSocialMedia, default);
			SerializationUtility.SerializeAtRevision(ref name, serializer, WorldRevision.ChatAndSocialMedia, default);
			SerializationUtility.SerializeAtRevision(ref description, serializer, WorldRevision.ChatAndSocialMedia, default);
			SerializationUtility.SerializeAtRevision(ref channelType, serializer, WorldRevision.ChatAndSocialMedia, default);
			SerializationUtility.SerializeAtRevision(ref guildId, serializer, WorldRevision.ChatAndSocialMedia, default);
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
	}
}