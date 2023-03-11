#nullable enable
using System.Linq;
using Core;
using Core.DataManagement;
using Core.WorldData.Data;
using UnityEngine;

namespace DevTools
{
	public class CreateForwardingRuleMenu : IDevMenu
	{
		private WorldManager world;
		private WorldLocalNetworkData lan;
		private WorldPortForwardingRule rule;
		private WorldComputerData[] computers;
		private int computerIndex = -1;
		private bool selectingComputer;

		/// <inheritdoc />
		public string Name => "Create LAN Forwarding Rule";

		public CreateForwardingRuleMenu(WorldManager world, WorldLocalNetworkData lan)
		{
			this.world = world;
			this.lan = lan;

			ObjectId[] computerIds = world.World.NetworkConnections.ToArray()
				.Where(x => x.LanId == lan.InstanceId)
				.Select(x => x.ComputerId)
				.Distinct()
				.ToArray();

			this.computers = this.world.World.Computers.ToArray()
				.Where(x => computerIds.Contains(x.InstanceId))
				.ToArray();
		}
		
		/// <inheritdoc />
		public void OnMenuGUI(DeveloperMenu devMenu)
		{
			if (selectingComputer)
			{
				for (var i = 0; i < computers.Length; i++)
				{
					if (!GUILayout.Button(computers[i].HostName))
						continue;

					computerIndex = i;
					selectingComputer = false;
					break;
				}
				
				return;
			}
			
			GUILayout.BeginHorizontal();
			GUILayout.Label("Inside Port:");
			GUILayout.FlexibleSpace();
			rule.LocalPort = DebugGUILayout.UShortField(rule.LocalPort);
			GUILayout.EndHorizontal();
			
			GUILayout.BeginHorizontal();
			GUILayout.Label("Outside Port:");
			GUILayout.FlexibleSpace();
			rule.GlobalPort = DebugGUILayout.UShortField(rule.GlobalPort);
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label("Computer:");
			GUILayout.FlexibleSpace();

			if (computerIndex == -1)
				GUILayout.Label("None");
			else GUILayout.Label(computers[computerIndex].HostName);

			if (GUILayout.Button("Select"))
				selectingComputer = true;

			GUILayout.EndHorizontal();

			if (computerIndex == -1)
				return;

			if (!GUILayout.Button("Create rule"))
				return;

			rule.LanId = lan.InstanceId;
			rule.ComputerId = computers[computerIndex].InstanceId;
			rule.InstanceId = world.GetNextObjectId();
			world.World.PortForwardingRules.Add(rule);

			devMenu.PopMenu();
		}
	}
}