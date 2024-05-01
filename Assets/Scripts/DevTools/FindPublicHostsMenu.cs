#nullable enable
using Core;
using System.Collections.Generic;
using System.Linq;
using Core.WorldData.Data;
using OS.Network;
using UnityEngine;

namespace DevTools
{
	public sealed class FindPublicHostsMenu : IDevMenu
	{
		private readonly WorldManager world;
		private bool refreshNeeded = true;
		private readonly Dictionary<ObjectId, string> ispsById = new Dictionary<ObjectId, string>();
		private readonly Dictionary<ObjectId, List<HostData>> hostsByIsp = new Dictionary<ObjectId, List<HostData>>();

		public FindPublicHostsMenu(WorldManager world)
		{
			this.world = world;
			this.Name = "Find Public Hosts";
		}
		
		/// <inheritdoc />
		public string Name { get; }

		/// <inheritdoc />
		public void OnMenuGUI(DeveloperMenu devMenu)
		{
			RefreshIfNeeded();

			if (GUILayout.Button("Refresh List"))
			{
				refreshNeeded = true;
				return;
			}

			foreach (ObjectId ispId in ispsById.Keys)
			{
				string ispName = ispsById[ispId];
				
				GUILayout.Label(ispName);

				foreach (HostData host in hostsByIsp[ispId])
				{
					GUILayout.BeginHorizontal();

					GUILayout.Label($"{host.deviceName} (Public IP: {host.ipAddress})");

					GUILayout.FlexibleSpace();

					if (GUILayout.Button("Copy IP Address"))
						GUIUtility.systemCopyBuffer = host.ipAddress;
                    
					GUILayout.EndHorizontal();
				}
			}
		}

		private void RefreshIfNeeded()
		{
			if (!refreshNeeded)
				return;

			refreshNeeded = false;

			ispsById.Clear();
			hostsByIsp.Clear();

			foreach (WorldInternetServiceProviderData isp in world.World.InternetProviders)
			{
				if (!NetUtility.TryParseCidrNotation(isp.CidrNetwork, out Subnet subnet))
					continue;
				
				ispsById[isp.InstanceId] = $"{isp.Name} ({isp.CidrNetwork})";

				var hosts = new List<HostData>();

				foreach (WorldLocalNetworkData network in world.World.LocalAreaNetworks.Where(x => x.ServiceProviderId == isp.InstanceId))
				{
					hosts.Add(new HostData
					{
						deviceName = network.Name,
						ipAddress = NetUtility.GetNetworkAddressString((subnet.networkAddress & subnet.mask) | (network.PublicNetworkAddress & ~subnet.mask))
					});
				}

				hostsByIsp[isp.InstanceId] = hosts;
			}
		}

		private struct HostData
		{
			public string deviceName;
			public string ipAddress;
		}
	}
}