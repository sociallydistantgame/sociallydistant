using ImGuiNET;
using SociallyDistant.Core;
using SociallyDistant.Core.Core;
using SociallyDistant.Core.Core.WorldData.Data;

namespace SociallyDistant.DevTools
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
			
			ImGui.Text("Computer");
			ImGui.Text("Inside Port");
			ImGui.Text("Outside Port");
			ImGui.Text("Actions");
			
			if (rules.Count == 0)
				ImGui.Text("No rules found");

			for (var i = 0; i < rules.Count; i++)
			{
				ImGui.Text(computerLookup[rules[i].ComputerId].HostName);
				ImGui.Text(rules[i].LocalPort.ToString());
				ImGui.Text(rules[i].GlobalPort.ToString());
				
				if (ImGui.Button("Delete"))
				{
					world.World.PortForwardingRules.Remove(rules[i]);

					ObjectId computer = rules[i].ComputerId;
					rules.RemoveAt(i);

					if (rules.All(x => x.ComputerId != computer))
						computerLookup.Remove(computer);
				}
			}

			if (ImGui.Button("Create new"))
			{
				devMenu.PushMenu(new CreateForwardingRuleMenu(world, lan));
				refreshOnNext = true;
			}
		}
	}
}