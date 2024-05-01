using Core.Serialization;

namespace Core.WorldData.Data
{
	public struct WorldRelationshipData :
		IWorldData,
		IDataWithId
	{
		private ObjectId id;
		private ObjectId source;
		private ObjectId target;
		private byte relationshipType;
		
		/// <inheritdoc />
		public void Serialize(IWorldSerializer serializer)
		{
			SerializationUtility.SerializeAtRevision(ref id, serializer, WorldRevision.ChatAndSocialMedia, default);
			SerializationUtility.SerializeAtRevision(ref source, serializer, WorldRevision.ChatAndSocialMedia, default);
			SerializationUtility.SerializeAtRevision(ref target, serializer, WorldRevision.ChatAndSocialMedia, default);
			SerializationUtility.SerializeAtRevision(ref relationshipType, serializer, WorldRevision.ChatAndSocialMedia, default);
		}

		/// <inheritdoc />
		public ObjectId InstanceId
		{
			get => id;
			set => id = value;
		}

		public ObjectId Source
		{
			get => source;
			set => source = value;
		}

		public ObjectId Target
		{
			get => target;
			set => target = value;
		}
		
		public RelationshipType Type
		{
			get => (RelationshipType) relationshipType;
			set => this.relationshipType = (byte) value;
		}
	}
}