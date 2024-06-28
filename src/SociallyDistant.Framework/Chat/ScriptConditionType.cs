#nullable enable
namespace SociallyDistant.Core.Chat
{
	public enum ScriptConditionType
	{
		Met,
		Unmet
	}

	public enum ScriptConditionCheck
	{
		MissionCompleted,
		InteractionCompleted,
		IsInMission,
		FriendsWith,
		BlockedBy,
		SeenProfile,
		SeenNetwork,
		SeenDevice,
		ReachedLevel,
		WorldFlag,
		Lifepath,
		MissionFailed
	}
}