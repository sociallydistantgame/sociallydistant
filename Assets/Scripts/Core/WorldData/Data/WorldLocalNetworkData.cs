#nullable enable
using Core.DataManagement;
using Core.Serialization;

namespace Core.WorldData.Data
{
	public struct WorldLocalNetworkData : 
		IWorldData, 
		IDataWithId
	{
		private ObjectId instanceId;
		private ObjectId ispId;
		private string name;

		/// <inheritdoc />
		public ObjectId InstanceId
		{
			get => instanceId;
			set => instanceId = value;
		}

		public ObjectId ServiceProviderId
		{
			get => ispId;
			set => ispId = value;
		}

		public string Name
		{
			get => name;
			set => name = value;
		}
		
		/// <inheritdoc />
		public void Serialize(IWorldSerializer serializer)
		{
			SerializationUtility.SerializeAtRevision(ref instanceId, serializer, WorldRevision.AddedInternetServiceProviders, default);
			SerializationUtility.SerializeAtRevision(ref ispId, serializer, WorldRevision.AddedInternetServiceProviders, default);
			SerializationUtility.SerializeAtRevision(ref name, serializer, WorldRevision.AddedInternetServiceProviders, default);
		}
	}
}