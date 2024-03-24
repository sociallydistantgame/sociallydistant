using Core.Serialization;

namespace Core.WorldData.Data
{
	public struct WorldDomainNameData :
		IWorldData,
		IDataWithId
	{
		private ObjectId instanceId;
		private string recordName;
		private uint ipAddress;

		/// <inheritdoc />
		public ObjectId InstanceId
		{
			get => instanceId;
			set => instanceId = value;
		}

		public string RecordName
		{
			get => recordName;
			set => recordName = value;
		}

		public uint Address
		{
			get => ipAddress;
			set => ipAddress = value;
		}
		
		/// <inheritdoc />
		public void Serialize(IWorldSerializer serializer)
		{
			SerializationUtility.SerializeAtRevision(ref instanceId, serializer, WorldRevision.DomainNames, default);
			SerializationUtility.SerializeAtRevision(ref recordName, serializer, WorldRevision.DomainNames, default);
			SerializationUtility.SerializeAtRevision(ref ipAddress, serializer, WorldRevision.DomainNames, default);
		}
	}
}