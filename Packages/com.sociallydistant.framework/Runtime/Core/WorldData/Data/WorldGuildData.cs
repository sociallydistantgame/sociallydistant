using Core.Serialization;

namespace Core.WorldData.Data
{
	public struct WorldGuildData :
		IWorldData,
		IDataWithId
	{
		private ObjectId id;
		private string name;
		
		
		/// <inheritdoc />
		public void Serialize(IWorldSerializer serializer)
		{
			SerializationUtility.SerializeAtRevision(ref id, serializer, WorldRevision.ChatAndSocialMedia, default);
			SerializationUtility.SerializeAtRevision(ref name, serializer, WorldRevision.ChatAndSocialMedia, default);
		}

		/// <inheritdoc />
		public ObjectId InstanceId
		{
			get => id;
			set => id = value;
		}

		public string Name
		{
			get => name;
			set => name = value;
		}
	}
}