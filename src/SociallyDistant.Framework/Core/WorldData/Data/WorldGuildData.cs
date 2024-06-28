using SociallyDistant.Core.Core.Serialization;

namespace SociallyDistant.Core.Core.WorldData.Data
{
	public struct WorldGuildData :
		IWorldData,
		IDataWithId,
		INarrativeObject
	{
		private ObjectId id;
		private string name;
		private string? narrativeId;
		
		
		/// <inheritdoc />
		public void Serialize(IWorldSerializer serializer)
		{
			SerializationUtility.SerializeAtRevision(ref id, serializer, WorldRevision.ChatAndSocialMedia, default);
			SerializationUtility.SerializeAtRevision(ref name, serializer, WorldRevision.ChatAndSocialMedia, default);
			SerializationUtility.SerializeAtRevision(ref narrativeId, serializer, WorldRevision.NarrativeGuilds, default);
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

		/// <inheritdoc />
		public string NarrativeId
		{
			get => narrativeId;
			set => narrativeId = value;
		}
	}
}