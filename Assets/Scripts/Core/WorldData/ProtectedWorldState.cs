#nullable enable
using Core.Serialization;

namespace Core.WorldData
{
	public struct ProtectedWorldState :
		IWorldData
	{
		private string lifepathId;

		public string LifepathId
		{
			get => lifepathId;
			set => lifepathId = value;
		}
		
		/// <inheritdoc />
		public void Serialize(IWorldSerializer serializer)
		{
			SerializationUtility.SerializeAtRevision(ref lifepathId, serializer, WorldRevision.Lifepaths, default);
		}
	}
}