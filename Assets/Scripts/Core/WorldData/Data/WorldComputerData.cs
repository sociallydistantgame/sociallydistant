#nullable enable

using Core.DataManagement;
using Core.Serialization;

namespace Core.WorldData.Data
{
	public struct WorldComputerData : ISerializable<WorldRevision>, IDataWithId
	{
		private ObjectId id;
		private string hostname;
		private long macAddress;

		/// <inheritdoc />
		public ObjectId InstanceId
		{
			get => id;
			set => id = value;
		}

		public string HostName
		{
			get => hostname;
			set => hostname = value;
		}

		public long MacAddress
		{
			get => macAddress;
			set => macAddress = value;
		}
		
		/// <inheritdoc />
		public void Serialize(IRevisionedSerializer<WorldRevision> serializer)
		{
			SerializationUtility.SerializeAtRevision(ref id, serializer, WorldRevision.AddedComputers, ObjectId.Invalid);
			SerializationUtility.SerializeAtRevision(ref hostname, serializer, WorldRevision.AddedComputers, "localhost");
			SerializationUtility.SerializeAtRevision(ref macAddress, serializer, WorldRevision.AddedMacAddresses, default);
		}
	}
}