#nullable enable
using System.Diagnostics;
using Serilog;
using SociallyDistant.Core.Core;
using SociallyDistant.Core.Core.WorldData.Data;
using SociallyDistant.GameplaySystems.NonPlayerComputers;
using SociallyDistant.Player;

namespace SociallyDistant.GameplaySystems.Networld
{
	public class NetworkEventListener
	{
		private readonly Dictionary<ObjectId, InternetServiceProvider> isps = new Dictionary<ObjectId, InternetServiceProvider>();
		private readonly Dictionary<ObjectId, LocalAreaNetwork> lans = new Dictionary<ObjectId, LocalAreaNetwork>();
		private readonly Dictionary<ObjectId, ForwardingTableEntry> portForwardEntries = new Dictionary<ObjectId, ForwardingTableEntry>();
		private NonPlayerComputerEventListener npcComputers = null!;
		private IWorldManager world = null!;

		
		private void Awake()
		{
			world = SociallyDistantGame.Instance.WorldManager;
		}

		private void Start()
		{
			InstallEvents();
		}

		private void OnDestroy()
		{
			UninstallEvents();
		}

		private void InstallEvents()
		{
			world.Callbacks.AddCreateCallback<WorldInternetServiceProviderData>(OnCreateIsp);
			world.Callbacks.AddDeleteCallback<WorldInternetServiceProviderData>(OnDeleteIsp);
			world.Callbacks.AddModifyCallback<WorldInternetServiceProviderData>(OnModifyIsp);
			
			world.Callbacks.AddCreateCallback<WorldLocalNetworkData>(OnCreateLAN);
			world.Callbacks.AddDeleteCallback<WorldLocalNetworkData>(OnDeleteLan);
			world.Callbacks.AddModifyCallback<WorldLocalNetworkData>(OnModifyLan);
			
			world.Callbacks.AddCreateCallback<WorldNetworkConnection>(OnCreateConnection);
			world.Callbacks.AddDeleteCallback<WorldNetworkConnection>(OnDeleteConnection);
			world.Callbacks.AddModifyCallback<WorldNetworkConnection>(OnModifyConnection);
			
			world.Callbacks.AddModifyCallback<WorldPlayerData>(OnPlayerDataModified);

			world.Callbacks.AddCreateCallback<WorldPortForwardingRule>(OnCreatePortForwardingRule);
			world.Callbacks.AddModifyCallback<WorldPortForwardingRule>(OnModifyPortForwardingRule);
			world.Callbacks.AddDeleteCallback<WorldPortForwardingRule>(OnDeletePortForwardingRule);
		}

		
		private void OnPlayerDataModified(WorldPlayerData subjectprevious, WorldPlayerData subjectnew)
		{
			if (isps.TryGetValue(subjectnew.PlayerInternetProvider, out InternetServiceProvider isp))
			{
			}
			else
			{
			}
		}

		private void UninstallEvents()
		{
			world.Callbacks.RemoveCreateCallback<WorldInternetServiceProviderData>(OnCreateIsp);
			world.Callbacks.RemoveDeleteCallback<WorldInternetServiceProviderData>(OnDeleteIsp);
			world.Callbacks.RemoveModifyCallback<WorldInternetServiceProviderData>(OnModifyIsp);
			
			world.Callbacks.RemoveCreateCallback<WorldLocalNetworkData>(OnCreateLAN);
			world.Callbacks.RemoveDeleteCallback<WorldLocalNetworkData>(OnDeleteLan);
			world.Callbacks.RemoveModifyCallback<WorldLocalNetworkData>(OnModifyLan);
			
			world.Callbacks.RemoveCreateCallback<WorldNetworkConnection>(OnCreateConnection);
			world.Callbacks.RemoveDeleteCallback<WorldNetworkConnection>(OnDeleteConnection);
			world.Callbacks.RemoveModifyCallback<WorldNetworkConnection>(OnModifyConnection);
			
			world.Callbacks.RemoveModifyCallback<WorldPlayerData>(OnPlayerDataModified);
			
			world.Callbacks.RemoveCreateCallback<WorldPortForwardingRule>(OnCreatePortForwardingRule);
			world.Callbacks.RemoveModifyCallback<WorldPortForwardingRule>(OnModifyPortForwardingRule);
			world.Callbacks.RemoveDeleteCallback<WorldPortForwardingRule>(OnDeletePortForwardingRule);
		}

		private void OnDeletePortForwardingRule(WorldPortForwardingRule subject)
		{
			if (!portForwardEntries.TryGetValue(subject.InstanceId, out ForwardingTableEntry entry))
				return;

			entry.Delete();
			portForwardEntries.Remove(subject.InstanceId);
		}

		private void OnModifyPortForwardingRule(WorldPortForwardingRule subjectprevious, WorldPortForwardingRule subjectnew)
		{
			// get the LAN
			if (!lans.TryGetValue(subjectnew.LanId, out LocalAreaNetwork lan))
				return;
			
			// and the computer
			if (!npcComputers.TryGetComputer(subjectnew.ComputerId, out NonPlayerComputer computer))
				return;
			
			// Computer must have a network
			if (computer.Network == null)
				return;
			
			// Computer must be connected to this LAN
			if (!lan.ContainsDevice(computer.Network))
				return;

			if (!portForwardEntries.TryGetValue(subjectnew.InstanceId, out ForwardingTableEntry entry))
				return;

			entry.Delete();
			portForwardEntries.Remove(subjectnew.InstanceId);

			OnCreatePortForwardingRule(subjectnew);
		}

		private void OnCreatePortForwardingRule(WorldPortForwardingRule subject)
		{
			// get the LAN
			if (!lans.TryGetValue(subject.LanId, out LocalAreaNetwork lan))
				return;
			
			// and the computer
			if (!npcComputers.TryGetComputer(subject.ComputerId, out NonPlayerComputer computer))
				return;
			
			// Computer must have a network
			if (computer.Network == null)
				return;
			
			// Computer must be connected to this LAN
			if (!lan.ContainsDevice(computer.Network))
				return;
			
			if (!portForwardEntries.TryGetValue(subject.InstanceId, out ForwardingTableEntry entry))
			{
				entry = lan.GetForwardingRule(computer.Network, subject.LocalPort, subject.GlobalPort);
				this.portForwardEntries.Add(subject.InstanceId, entry);
			}
		}

		
		private void OnModifyConnection(WorldNetworkConnection subjectprevious, WorldNetworkConnection subjectnew)
		{
			// If nothing's changed then ignore the Modify event. Otherwise we'll needlessly wreak havoc in the simulation.
			if (subjectprevious.ComputerId == subjectnew.ComputerId && subjectprevious.LanId == subjectnew.LanId)
			{
				Log.Warning("Something caused a WorldNetworkConnection ModifyEvent but didn't actually change anything. Can we not? Please? Y'all are lucky Ritchie programs defensively.");
				return;
			}

			HandleConnect(subjectnew);
		}

		private void OnDeleteConnection(WorldNetworkConnection subject)
		{
			HandleDisconnect(subject);
		}

		private void OnCreateConnection(WorldNetworkConnection subject)
		{
			HandleConnect(subject);
		}

		private void HandleDisconnect(WorldNetworkConnection data)
		{
			if (npcComputers.TryGetComputer(data.ComputerId, out NonPlayerComputer npc))
				npc.DisconnectLan();
		}
		
		private void HandleConnect(WorldNetworkConnection data)
		{
			// Find a computer
			if (!npcComputers.TryGetComputer(data.ComputerId, out NonPlayerComputer computer))
				return;
			
			// Handle LAN changes
			if (!lans.TryGetValue(data.LanId, out LocalAreaNetwork lan))
				computer.DisconnectLan();
			else
				computer.ConnectLan(lan);
		}
		
		private void OnModifyLan(WorldLocalNetworkData subjectprevious, WorldLocalNetworkData subjectnew)
		{
			if (!lans.TryGetValue(subjectnew.InstanceId, out LocalAreaNetwork net))
				return;

			UpdateLAN(net, subjectnew);
		}

		private void OnDeleteLan(WorldLocalNetworkData subject)
		{
			if (!lans.TryGetValue(subject.InstanceId, out LocalAreaNetwork lan))
				return;

			if (lan.InternetServiceProvider != null)
				lan.DisconnectFromInternet();

			lans.Remove(subject.InstanceId);
			//simulation.DeleteLan(lan);
		}

		private void OnCreateLAN(WorldLocalNetworkData subject)
		{
			if (!lans.TryGetValue(subject.InstanceId, out LocalAreaNetwork lan))
			{
				//lan = simulation.CreateLocalAreaNetwork();
				//lans.Add(subject.InstanceId, lan);
			}

			UpdateLAN(lan, subject);
		}

		private void OnModifyIsp(WorldInternetServiceProviderData subjectprevious, WorldInternetServiceProviderData subjectnew)
		{
			
		}

		private void OnDeleteIsp(WorldInternetServiceProviderData subject)
		{
			
		}

		private void OnCreateIsp(WorldInternetServiceProviderData subject)
		{
			if (!isps.TryGetValue(subject.InstanceId, out InternetServiceProvider isp))
			{
				//isp = simulation.Value.CreateInternetServiceProvider(subject.CidrNetwork);
				//isps.Add(subject.InstanceId, isp);
			}

			UpdateInstance(isp, subject);
		}

		private void UpdateInstance(InternetServiceProvider isp, WorldInternetServiceProviderData data)
		{
			// During game load, this ensures we connect the player to the right ISP when the ISP is loaded.
			WorldPlayerData player = this.world.World.PlayerData.Value;
			if (player.PlayerInternetProvider == data.InstanceId)
				this.OnPlayerDataModified(player, player);
		}

		private void UpdateLAN(LocalAreaNetwork lan, WorldLocalNetworkData data)
		{
			if (!isps.TryGetValue(data.ServiceProviderId, out InternetServiceProvider isp))
			{
				lan.DisconnectFromInternet();
			}
			else
			{
				if (!isp.IsConnectedWithLAN(lan))
				{
					lan.DisconnectFromInternet();
					lan.ConnectToInternet(isp, data.PublicNetworkAddress);
				}
			}
			
		}
	}

	public class MalwareState
	{
		
	}
}