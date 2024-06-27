using System.Collections.Generic;
using System.Linq;
using Core;
using Core.DataManagement;
using Core.WorldData.Data;
using UnityEngine;

namespace DevTools
{
	public class ManageLanRulesMenu : IDevMenu
	{
		private WorldManager world;
		private WorldLocalNetworkData lan;
		private List<WorldPortForwardingRule> rules;
		private Dictionary<ObjectId, WorldComputerData> computerLookup;
		private bool refreshOnNext;

		public string Name => $"Manage rules for LAN: {lan.Name}";

		public ManageLanRulesMenu(WorldManager world, WorldLocalNetworkData lan)
		{
			this.world = world;
			this.lan = lan;
			Refresh();
		}

		private void Refresh()
		{
			this.rules = this.world.World.PortForwardingRules.ToArray()
				.Where(x => x.LanId == lan.InstanceId)
				.ToList();

			this.computerLookup = this.world.World.Computers.ToArray()
				.Where(x => this.rules.Any(y => y.ComputerId == x.InstanceId))
				.ToDictionary(x => x.InstanceId);

			refreshOnNext = false;
		}

		public void OnMenuGUI(DeveloperMenu devMenu)
		{
			if (refreshOnNext)
				Refresh();
			
			GUILayout.BeginHorizontal();
			GUILayout.Label("Computer");
			GUILayout.FlexibleSpace();
			GUILayout.Label("Inside Port");
			GUILayout.FlexibleSpace();
			GUILayout.Label("Outside Port");
			GUILayout.FlexibleSpace();
			GUILayout.Label("Actions");
			GUILayout.EndHorizontal();

			if (rules.Count == 0)
				GUILayout.Label("No rules found");

			for (var i = 0; i < rules.Count; i++)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label(computerLookup[rules[i].ComputerId].HostName);
				GUILayout.FlexibleSpace();
				GUILayout.Label(rules[i].LocalPort.ToString());
				GUILayout.FlexibleSpace();
				GUILayout.Label(rules[i].GlobalPort.ToString());
				GUILayout.FlexibleSpace();

				if (GUILayout.Button("Delete"))
				{
					world.World.PortForwardingRules.Remove(rules[i]);

					ObjectId computer = rules[i].ComputerId;
					rules.RemoveAt(i);

					if (rules.All(x => x.ComputerId != computer))
						computerLookup.Remove(computer);
				}
				
				GUILayout.EndHorizontal();
			}

			if (GUILayout.Button("Create new"))
			{
				devMenu.PushMenu(new CreateForwardingRuleMenu(world, lan));
				refreshOnNext = true;
			}
		}
	}
}