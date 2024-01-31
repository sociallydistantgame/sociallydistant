#nullable enable
using System;
using System.Collections.Generic;
using Core;
using Core.DataManagement;
using Core.WorldData.Data;
using GameplaySystems.NonPlayerComputers;
using Player;
using UnityEngine;
using UnityExtensions;
using Utility;

namespace GameplaySystems.Networld
{
	public class NetworkEventListener : MonoBehaviour
	{
		private readonly Dictionary<ObjectId, InternetServiceProvider> isps = new Dictionary<ObjectId, InternetServiceProvider>();
		private readonly Dictionary<ObjectId, LocalAreaNetwork> lans = new Dictionary<ObjectId, LocalAreaNetwork>();
		private readonly Dictionary<ObjectId, ForwardingTableEntry> portForwardEntries = new Dictionary<ObjectId, ForwardingTableEntry>();
		private NonPlayerComputerEventListener npcComputers = null!;

		[Header("Dependencies")]
		[SerializeField]
		private PlayerInstanceHolder playerInstance = null!;
		
		[SerializeField]
		private WorldManagerHolder world = null!;

		[SerializeField]
		private NetworkSimulationHolder simulation = null!;

		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(NetworkEventListener));
		}

		private void Start()
		{
			UnityHelpers.MustFindObjectOfType(out npcComputers);

			InstallEvents();
		}

		private void OnDestroy()
		{
			UninstallEvents();
		}

		private void InstallEvents()
		{
			world.Value.Callbacks.AddCreateCallback<WorldInternetServiceProviderData>(OnCreateIsp);
			world.Value.Callbacks.AddDeleteCallback<WorldInternetServiceProviderData>(OnDeleteIsp);
			world.Value.Callbacks.AddModifyCallback<WorldInternetServiceProviderData>(OnModifyIsp);
			
			world.Value.Callbacks.AddCreateCallback<WorldLocalNetworkData>(OnCreateLAN);
			world.Value.Callbacks.AddDeleteCallback<WorldLocalNetworkData>(OnDeleteLan);
			world.Value.Callbacks.AddModifyCallback<WorldLocalNetworkData>(OnModifyLan);
			
			world.Value.Callbacks.AddCreateCallback<WorldNetworkConnection>(OnCreateConnection);
			world.Value.Callbacks.AddDeleteCallback<WorldNetworkConnection>(OnDeleteConnection);
			world.Value.Callbacks.AddModifyCallback<WorldNetworkConnection>(OnModifyConnection);
			
			world.Value.Callbacks.AddModifyCallback<WorldPlayerData>(OnPlayerDataModified);

			world.Value.Callbacks.AddCreateCallback<WorldPortForwardingRule>(OnCreatePortForwardingRule);
			world.Value.Callbacks.AddModifyCallback<WorldPortForwardingRule>(OnModifyPortForwardingRule);
			world.Value.Callbacks.AddDeleteCallback<WorldPortForwardingRule>(OnDeletePortForwardingRule);
		}

		
		private void OnPlayerDataModified(WorldPlayerData subjectprevious, WorldPlayerData subjectnew)
		{
			if (isps.TryGetValue(subjectnew.PlayerInternetProvider, out InternetServiceProvider isp))
			{
				if (playerInstance.Value.PlayerLan.InternetServiceProvider == isp)
					return;
				
				playerInstance.Value.PlayerLan.DisconnectFromInternet();
				playerInstance.Value.PlayerLan.ConnectToInternet(isp);
			}
			else
			{
				playerInstance.Value.PlayerLan.DisconnectFromInternet();
			}
		}

		private void UninstallEvents()
		{
			world.Value.Callbacks.RemoveCreateCallback<WorldInternetServiceProviderData>(OnCreateIsp);
			world.Value.Callbacks.RemoveDeleteCallback<WorldInternetServiceProviderData>(OnDeleteIsp);
			world.Value.Callbacks.RemoveModifyCallback<WorldInternetServiceProviderData>(OnModifyIsp);
			
			world.Value.Callbacks.RemoveCreateCallback<WorldLocalNetworkData>(OnCreateLAN);
			world.Value.Callbacks.RemoveDeleteCallback<WorldLocalNetworkData>(OnDeleteLan);
			world.Value.Callbacks.RemoveModifyCallback<WorldLocalNetworkData>(OnModifyLan);
			
			world.Value.Callbacks.RemoveCreateCallback<WorldNetworkConnection>(OnCreateConnection);
			world.Value.Callbacks.RemoveDeleteCallback<WorldNetworkConnection>(OnDeleteConnection);
			world.Value.Callbacks.RemoveModifyCallback<WorldNetworkConnection>(OnModifyConnection);
			
			world.Value.Callbacks.RemoveModifyCallback<WorldPlayerData>(OnPlayerDataModified);
			
			world.Value.Callbacks.RemoveCreateCallback<WorldPortForwardingRule>(OnCreatePortForwardingRule);
			world.Value.Callbacks.RemoveModifyCallback<WorldPortForwardingRule>(OnModifyPortForwardingRule);
			world.Value.Callbacks.RemoveDeleteCallback<WorldPortForwardingRule>(OnDeletePortForwardingRule);
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
			
			// Get a LAN.
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
			simulation.Value.DeleteLan(lan);
		}

		private void OnCreateLAN(WorldLocalNetworkData subject)
		{
			if (!lans.TryGetValue(subject.InstanceId, out LocalAreaNetwork lan))
			{
				lan = simulation.Value.CreateLocalAreaNetwork();
				lans.Add(subject.InstanceId, lan);
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
				isp = simulation.Value.CreateInternetServiceProvider(subject.CidrNetwork);
				isps.Add(subject.InstanceId, isp);
			}

			UpdateInstance(isp, subject);
		}

		private void UpdateInstance(InternetServiceProvider isp, WorldInternetServiceProviderData data)
		{
			
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
					lan.ConnectToInternet(isp);
				}
			}
			
		}
	}

	public class MalwareState
	{
		
	}
}