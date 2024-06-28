#nullable enable
using SociallyDistant.Core.Core.Serialization;

namespace SociallyDistant.Core.Core.WorldData.Data
{
	public struct WorldInternetServiceProviderData : 
		IWorldData, 
		INarrativeObject,
		IDataWithId
	{
		private ObjectId instanceId;
		private string name;
		private string narrativeId;
		private string cidrNetwork;
		
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
		
		public string Name
		{
			get => name;
			set => name = value;
		}

		public string CidrNetwork
		{
			get => cidrNetwork;
			set => cidrNetwork = value;
		}
		
		/// <inheritdoc />
		public void Serialize(IWorldSerializer serializer)
		{
			SerializationUtility.SerializeAtRevision(ref instanceId, serializer, WorldRevision.AddedInternetServiceProviders, default);
			SerializationUtility.SerializeAtRevision(ref narrativeId, serializer, WorldRevision.LocalNetworkNarrativeIds, default);
			SerializationUtility.SerializeAtRevision(ref name, serializer, WorldRevision.AddedInternetServiceProviders, default);
			SerializationUtility.SerializeAtRevision(ref cidrNetwork, serializer, WorldRevision.AddedInternetServiceProviders, default);
		}
	}
}