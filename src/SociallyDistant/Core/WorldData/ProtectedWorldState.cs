#nullable enable
using SociallyDistant.Core.Core;
using SociallyDistant.Core.Core.Serialization;

namespace SociallyDistant.Core.WorldData
{
	public struct ProtectedWorldState :
		IWorldData
	{
		private string lifepathId;
		private string gameVersion;
		private string currentMissionId;
		private IReadOnlyList<string> completedMissions;
		private IReadOnlyList<string> completedInteractions;
		private IReadOnlyList<string> failedMissions;
		private ulong experience;

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

		public string CurrentMissionId
		{
			get => currentMissionId;
			set => currentMissionId = value;
		}

		public IReadOnlyList<string> FailedMissions
		{
			get => failedMissions;
			set => failedMissions = value;
		}
		
		public IReadOnlyList<string> CompletedMissions
		{
			get => completedMissions;
			set => completedMissions = value;
		}
		
		public IReadOnlyList<string> CompletedInteractions
		{
			get => completedInteractions;
			set => completedInteractions = value;
		}

		public ulong Experience
		{
			get => experience;
			set => experience = value;
		}
		
		/// <inheritdoc />
		public void Serialize(IWorldSerializer serializer)
		{
			SerializationUtility.SerializeAtRevision(ref lifepathId, serializer, WorldRevision.Lifepaths, default);
			SerializationUtility.SerializeAtRevision(ref gameVersion, serializer, WorldRevision.ChangeLogs, default);
			SerializationUtility.SerializeAtRevision(ref currentMissionId, serializer, WorldRevision.MissionFailures, default);
			SerializationUtility.SerializeAtRevision(ref experience, serializer, WorldRevision.MissionFailures, default);
			
			SerializationUtility.SerializeCollectionAtRevision(ref completedMissions, serializer, WorldRevision.CompletedMissions);
			SerializationUtility.SerializeCollectionAtRevision(ref completedInteractions, serializer, WorldRevision.CompletedInteractions);
			SerializationUtility.SerializeCollectionAtRevision(ref failedMissions, serializer, WorldRevision.MissionFailures);
			
		}
	}
}