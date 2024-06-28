using SociallyDistant.Core.Core.Serialization;

namespace SociallyDistant.Core.Core.WorldData.Data
{
	public struct WorldWitnessedObjectData :
		IWorldData,
		IDataWithId
	{
		private ObjectId id;
		private ObjectId witnessedId;
		private byte objectType;
		
		/// <inheritdoc />
		public void Serialize(IWorldSerializer serializer)
		{
			SerializationUtility.SerializeAtRevision(ref id, serializer, WorldRevision.MissionFailures, default);
			SerializationUtility.SerializeAtRevision(ref witnessedId, serializer, WorldRevision.MissionFailures, default);
			SerializationUtility.SerializeAtRevision(ref objectType, serializer, WorldRevision.MissionFailures, default);
			
		}

		/// <inheritdoc />
		public ObjectId InstanceId
		{
			get => id;
			set => id = value;
		}

		public ObjectId WitnessedObject
		{
			get => witnessedId;
			set => witnessedId = value;
		}

		public WitnessedObjectType Type
		{
			get => (WitnessedObjectType) this.objectType;
			set => this.objectType = (byte) value;
		}
	}
}