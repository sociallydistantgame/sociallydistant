#nullable enable

using SociallyDistant.Core.Core.Serialization;

namespace SociallyDistant.Core.Core.WorldData.Data
{
	public struct WorldComputerData : 
		IWorldData,
		INarrativeObject,
		IDataWithId
	{
		private ObjectId id;
		private string hostname;
		private long macAddress;
		private string narrativeId;
		private IReadOnlyList<NetworkServiceData> services;

		/// <inheritdoc />
		public ObjectId InstanceId
		{
			get => id;
			set => id = value;
		}

		public string NarrativeId
		{
			get => narrativeId;
			set => narrativeId = value;
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

		public IReadOnlyList<NetworkServiceData> Services
		{
			get
			{
				if (services == null)
					services = new List<NetworkServiceData>();

				return services;
			}
			set => services = value;
		}

		/// <inheritdoc />
		public void Serialize(IWorldSerializer serializer)
		{
			SerializationUtility.SerializeAtRevision(ref id, serializer, WorldRevision.AddedComputers, ObjectId.Invalid);
			SerializationUtility.SerializeAtRevision(ref narrativeId, serializer, WorldRevision.LocalNetworkNarrativeIds, default);
			SerializationUtility.SerializeAtRevision(ref hostname, serializer, WorldRevision.AddedComputers, "localhost");
			SerializationUtility.SerializeAtRevision(ref macAddress, serializer, WorldRevision.AddedMacAddresses, default);

			SerializationUtility.SerializeCollectionAtRevision(ref services, serializer, WorldRevision.ComputersCanListen);
		}
	}
}