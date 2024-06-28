#nullable enable
using SociallyDistant.Core.Core.Serialization;

namespace SociallyDistant.Core.Core.WorldData.Data
{
	public struct WorldPlayerData : IWorldData
	{
		private ObjectId playerIsp;
		private ObjectId playerProfile;
		private uint publicNetworkAddress;

		public uint PublicNetworkAddress
		{
			get => publicNetworkAddress;
			set => publicNetworkAddress = value;
		}
		
		public ObjectId PlayerProfile
		{
			get => playerProfile;
			set => playerProfile = value;
		}
		
		public ObjectId PlayerInternetProvider
		{
			get => playerIsp;
			set => playerIsp = value;
		}
		
		/// <inheritdoc />
		public void Serialize(IWorldSerializer serializer)
		{
			SerializationUtility.SerializeAtRevision(ref playerIsp, serializer, WorldRevision.AddedInternetServiceProviders, default);
			SerializationUtility.SerializeAtRevision(ref playerProfile, serializer, WorldRevision.ChatAndSocialMedia, default);
			SerializationUtility.SerializeAtRevision(ref publicNetworkAddress, serializer, WorldRevision.PlayerIpStoredInSave, default);
		}
	}
}