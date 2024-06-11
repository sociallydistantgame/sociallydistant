#nullable enable
using Core.Serialization;
using Core.WorldData.Data;

namespace Core
{
	/// <summary>
	///		Represents a Socially Distant world.
	/// </summary>
	public interface IWorld
	{
		string GameVersion { get; }
		
		IWorldDataObject<GlobalWorldData> GlobalWorldState { get; }
		IWorldDataObject<WorldPlayerData> PlayerData { get; }
		IWorldTable<WorldComputerData> Computers { get; }
		IWorldTable<WorldInternetServiceProviderData> InternetProviders { get; }
		IWorldTable<WorldLocalNetworkData> LocalAreaNetworks { get; }
		IWorldTable<WorldNetworkConnection> NetworkConnections { get; }
		IWorldTable<WorldPortForwardingRule> PortForwardingRules { get; }
		IWorldTable<WorldCraftedExploitData> CraftedExploits { get; }
		IWorldTable<WorldHackableData> Hackables { get; }
		INarrativeObjectTable<WorldProfileData> Profiles { get; }
		INarrativeObjectTable<WorldPostData> Posts { get; }
		IWorldTable<WorldWitnessedObjectData> WitnessedObjects { get; }
		IWorldTable<WorldMessageData> Messages { get; }
		INarrativeObjectTable<WorldChannelData> Channels { get; }
		INarrativeObjectTable<WorldGuildData> Guilds { get; }
		IWorldTable<WorldMemberData> Members { get; }
		IWorldTable<WorldRelationshipData> Relationships { get; }
		IWorldTable<WorldDomainNameData> Domains { get; }
		INarrativeObjectTable<WorldMailData> Emails { get; }
		
		
		IWorldFlagCollection WorldFlags { get; }

		ulong PlayerExperience { get; }
		string CurrentMissionId { get; }
		string NarrativeLifePath { get; }
		bool IsMissionCompleted(string missionId);
		bool IsInteractionCompleted(string interactionId);
		bool WasMissionFailed(string missionId);

		void Serialize(IWorldSerializer serializer);
	}
}