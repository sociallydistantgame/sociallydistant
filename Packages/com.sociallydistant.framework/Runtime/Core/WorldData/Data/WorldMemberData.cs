using Core.Serialization;

namespace Core.WorldData.Data
{
	public struct WorldMemberData :
		IWorldData,
		IDataWithId
	{
		private ObjectId id;
		private ObjectId profileId;
		private ObjectId groupId;
		private byte groupType;
		
		/// <inheritdoc />
		public void Serialize(IWorldSerializer serializer)
		{
			SerializationUtility.SerializeAtRevision(ref id, serializer, WorldRevision.ChatAndSocialMedia, default);
			SerializationUtility.SerializeAtRevision(ref profileId, serializer, WorldRevision.ChatAndSocialMedia, default);
			SerializationUtility.SerializeAtRevision(ref groupId, serializer, WorldRevision.ChatAndSocialMedia, default);
			SerializationUtility.SerializeAtRevision(ref groupType, serializer, WorldRevision.ChatAndSocialMedia, default);
		}
		
		/// <inheritdoc />
		public ObjectId InstanceId
		{
			get => id;
			set => id = value;
		}

		public ObjectId ProfileId
		{
			get => profileId;
			set => profileId = value;
		}

		public ObjectId GroupId
		{
			get => groupId;
			set => groupId = value;
		}

		public MemberGroupType GroupType
		{
			get => (MemberGroupType) groupType;
			set => groupType = (byte) value;
		}
	}
}