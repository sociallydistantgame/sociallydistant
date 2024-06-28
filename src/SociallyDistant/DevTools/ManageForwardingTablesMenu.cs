#nullable enable
using ImGuiNET;
using SociallyDistant.Core;
using SociallyDistant.Core.Core.WorldData.Data;

namespace SociallyDistant.DevTools
{
	public class ManageForwardingTablesMenu : IDevMenu
	{
		private WorldManager world;
		private WorldLocalNetworkData[] lans;
		
		/// <inheritdoc />
		public string Name => "Manage Forwarding Tables";

		public ManageForwardingTablesMenu()
		{
			this.world = WorldManager.Instance;
			this.lans = world.World.LocalAreaNetworks.ToArray();
		}
		
		/// <inheritdoc />
		public void OnMenuGUI(DeveloperMenu devMenu)
		{
			ImGui.Text("Select LAN to edit");

			for (var i = 0; i < lans.Length; i++)
			{
				if (!ImGui.Button(lans[i].Name))
					continue;

				devMenu.PushMenu(new ManageLanRulesMenu(world, lans[i]));
			}
		}
	}
}