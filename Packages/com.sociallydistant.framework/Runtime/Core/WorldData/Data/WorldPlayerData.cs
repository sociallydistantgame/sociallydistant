#nullable enable
using Core.Serialization;

namespace Core.WorldData.Data
{
	public struct WorldPlayerData : IWorldData
	{
		private ObjectId playerIsp;
		private ObjectId playerProfile;

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
		}
	}
}