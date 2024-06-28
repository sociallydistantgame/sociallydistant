#nullable enable
using SociallyDistant.Core.Core.Serialization;

namespace SociallyDistant.Core.Core.WorldData.Data
{
	public struct WorldNetworkConnection : 
		IWorldData, 
		IDataWithId
	{
		private ObjectId instanceId;
		private ObjectId computerId;
		private ObjectId lanId;

		/// <inheritdoc />
		public ObjectId InstanceId
		{
			get => instanceId;
			set => instanceId = value;
		}

		public ObjectId ComputerId
		{
			get => computerId;
			set => computerId = value;
		}

		public ObjectId LanId
		{
			get => lanId;
			set => lanId = value;
		}

		/// <inheritdoc />
		public void Serialize(IWorldSerializer serializer)
		{
			SerializationUtility.SerializeAtRevision(ref instanceId, serializer, WorldRevision.AddedInternetServiceProviders, default);
			SerializationUtility.SerializeAtRevision(ref computerId, serializer, WorldRevision.AddedInternetServiceProviders, default);
			SerializationUtility.SerializeAtRevision(ref lanId, serializer, WorldRevision.AddedInternetServiceProviders, default);
		}
	}
}