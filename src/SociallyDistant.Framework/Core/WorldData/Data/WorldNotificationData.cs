using SociallyDistant.Core.Core.Serialization;

namespace SociallyDistant.Core.Core.WorldData.Data
{
	public struct WorldNotificationData :
		IWorldData,
		IDataWithId
	{
		private ObjectId id;
		private string groupId;
		private ObjectId correlationId;

		/// <inheritdoc />
		public ObjectId InstanceId
		{
			get => id;
			set => id = value;
		}

		public ObjectId CorrelationId
		{
			get => correlationId;
			set => correlationId = value;
		}

		public string GroupId
		{
			get => groupId;
			set => groupId = value;
		}
		
		/// <inheritdoc />
		public void Serialize(IWorldSerializer serializer)
		{
			SerializationUtility.SerializeAtRevision(ref id, serializer, WorldRevision.Notifications, default);
			SerializationUtility.SerializeAtRevision(ref correlationId, serializer, WorldRevision.Notifications, default);
			SerializationUtility.SerializeAtRevision(ref groupId, serializer, WorldRevision.Notifications, default);
		}
	}
}