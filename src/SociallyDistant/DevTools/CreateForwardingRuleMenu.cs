#nullable enable
using ImGuiNET;
using SociallyDistant.Core;
using SociallyDistant.Core.Core;
using SociallyDistant.Core.Core.WorldData.Data;

namespace SociallyDistant.DevTools
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
					if (!ImGui.Button(computers[i].HostName))
						continue;

					computerIndex = i;
					selectingComputer = false;
					break;
				}
				
				return;
			}
			
			ImGui.Text("Inside Port:");
			rule.LocalPort = DebugGUILayout.UShortField(rule.LocalPort);
			
			ImGui.Text("Outside Port:");
			rule.GlobalPort = DebugGUILayout.UShortField(rule.GlobalPort);
			
			ImGui.Text("Computer:");

			if (computerIndex == -1)
				ImGui.Text("None");
			else ImGui.Text(computers[computerIndex].HostName);

			if (ImGui.Button("Select"))
				selectingComputer = true;

			if (computerIndex == -1)
				return;

			if (!ImGui.Button("Create rule"))
				return;

			rule.LanId = lan.InstanceId;
			rule.ComputerId = computers[computerIndex].InstanceId;
			rule.InstanceId = world.GetNextObjectId();
			world.World.PortForwardingRules.Add(rule);

			devMenu.PopMenu();
		}
	}
}