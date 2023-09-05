#nullable enable
using Core.Serialization;

namespace Core.WorldData.Data
{
	public struct WorldPlayerData : IWorldData
	{
		private ObjectId playerIsp;

		public ObjectId PlayerInternetProvider
		{
			get => playerIsp;
			set => playerIsp = value;
		}
		
		/// <inheritdoc />
		public void Serialize(IWorldSerializer serializer)
		{
			SerializationUtility.SerializeAtRevision(ref playerIsp, serializer, WorldRevision.AddedInternetServiceProviders, default);
		}
	}
}