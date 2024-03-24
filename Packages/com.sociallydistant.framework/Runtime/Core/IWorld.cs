#nullable enable
using System.Runtime.InteropServices.ComTypes;
using Core.WorldData;
using Core.WorldData.Data;

namespace Core
{
	/// <summary>
	///		Represents a Socially Distant world.
	/// </summary>
	public interface IWorld
	{
		IWorldDataObject<GlobalWorldData> GlobalWorldState { get; }
		IWorldDataObject<WorldPlayerData> PlayerData { get; }
		IWorldTable<WorldComputerData> Computers { get; }
		IWorldTable<WorldInternetServiceProviderData> InternetProviders { get; }
		IWorldTable<WorldLocalNetworkData> LocalAreaNetworks { get; }
		IWorldTable<WorldNetworkConnection> NetworkConnections { get; }
		IWorldTable<WorldPortForwardingRule> PortForwardingRules { get; }
		IWorldTable<WorldCraftedExploitData> CraftedExploits { get; }
		IWorldTable<WorldHackableData> Hackables { get; }
		IWorldTable<WorldProfileData> Profiles { get; }
		IWorldTable<WorldPostData> Posts { get; }
		IWorldTable<WorldMessageData> Messages { get; }
		IWorldTable<WorldChannelData> Channels { get; }
		IWorldTable<WorldGuildData> Guilds { get; }
		IWorldTable<WorldMemberData> Members { get; }
		IWorldTable<WorldRelationshipData> Relationships { get; }
		IWorldTable<WorldDomainNameData> Domains { get; }
		
		
		IWorldFlagCollection WorldFlags { get; }
	}
}