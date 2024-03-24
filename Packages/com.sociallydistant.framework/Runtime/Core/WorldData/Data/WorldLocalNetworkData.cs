#nullable enable
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
		private string narrativeId;
		private uint publicIpAddress;

		/// <inheritdoc />
		public ObjectId InstanceId
		{
			get => instanceId;
			set => instanceId = value;
		}

		public string NarrativeId
		{
			get => narrativeId;
			set => narrativeId = value;
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

		public uint PublicNetworkAddress
		{
			get => publicIpAddress;
			set => publicIpAddress = value;
		}
		
		/// <inheritdoc />
		public void Serialize(IWorldSerializer serializer)
		{
			SerializationUtility.SerializeAtRevision(ref instanceId, serializer, WorldRevision.AddedInternetServiceProviders, default);
			SerializationUtility.SerializeAtRevision(ref narrativeId, serializer, WorldRevision.LocalNetworkNarrativeIds, default);
			SerializationUtility.SerializeAtRevision(ref ispId, serializer, WorldRevision.AddedInternetServiceProviders, default);
			SerializationUtility.SerializeAtRevision(ref name, serializer, WorldRevision.AddedInternetServiceProviders, default);
			SerializationUtility.SerializeAtRevision(ref publicIpAddress, serializer, WorldRevision.PublicIpsStoredInSave, default);
		}
	}
}