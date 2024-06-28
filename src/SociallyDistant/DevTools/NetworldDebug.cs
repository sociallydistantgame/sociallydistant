#nullable enable
using ImGuiNET;
using SociallyDistant.Core;
using SociallyDistant.GameplaySystems.Networld;

namespace SociallyDistant.DevTools
{
	public class NetworldDebug : IDevMenu
	{
		private NetworkSimulationController simulation;
		private WorldManager world;
		
		public string Name => "Networld Debugger";

		public NetworldDebug(NetworkSimulationController simulation)
		{
			this.world = world;
			this.simulation = simulation;
		}
		
		/// <inheritdoc />
		public void OnMenuGUI(DeveloperMenu devMenu)
		{
			if (ImGui.Button("Set Player's ISP"))
				devMenu.PushMenu(new SetPlayerIspMenu());
			
			if (ImGui.Button("Create ISP"))
				devMenu.PushMenu(new CreateIspMenu(world));

			if (ImGui.Button("Create LAN"))
				devMenu.PushMenu(new CreateLanMenu(world));
			
			if (ImGui.Button("Create Computer"))
				devMenu.PushMenu(new CreateComputerMenu(world));
			
			if (ImGui.Button("Set LAN for Computer"))
				devMenu.PushMenu(new SetComputerLANMenu(world));
			
			if (ImGui.Button("Manage LAN Forwarding Tables"))
				devMenu.PushMenu(new ManageForwardingTablesMenu());
			
			if (ImGui.Button("Find Public Hosts"))
				devMenu.PushMenu(new FindPublicHostsMenu(world));
		}
	}
}