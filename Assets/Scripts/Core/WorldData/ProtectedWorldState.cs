#nullable enable
using System.Collections.Generic;
using Core.Serialization;

namespace Core.WorldData
{
	public struct ProtectedWorldState :
		IWorldData
	{
		private string lifepathId;
		private string gameVersion;
		private IReadOnlyList<string> completedMissions;

		public string GameVersion
		{
			get => gameVersion;
			set => gameVersion = value;
		}

		public string LifepathId
		{
			get => lifepathId;
			set => lifepathId = value;
		}

		public IReadOnlyList<string> CompletedMissions
		{
			get => completedMissions;
			set => completedMissions = value;
		}
		
		/// <inheritdoc />
		public void Serialize(IWorldSerializer serializer)
		{
			SerializationUtility.SerializeAtRevision(ref lifepathId, serializer, WorldRevision.Lifepaths, default);
			SerializationUtility.SerializeAtRevision(ref gameVersion, serializer, WorldRevision.ChangeLogs, default);
			
			SerializationUtility.SerializeCollectionAtRevision(ref completedMissions, serializer, WorldRevision.CompletedMissions);
		}
	}
}