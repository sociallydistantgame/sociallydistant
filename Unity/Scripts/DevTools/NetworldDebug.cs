#nullable enable
using Core;
using Core.WorldData;
using GameplaySystems.Networld;
using Player;
using UnityEngine;

namespace DevTools
{
	public class NetworldDebug : IDevMenu
	{
		private NetworkSimulationController simulation;
		private WorldManager world;
		private PlayerInstanceHolder player;
		
		public string Name => "Networld Debugger";

		public NetworldDebug(NetworkSimulationController simulation, PlayerInstanceHolder playerInstance)
		{
			this.world = world;
			this.simulation = simulation;
			this.player = playerInstance;
		}
		
		/// <inheritdoc />
		public void OnMenuGUI(DeveloperMenu devMenu)
		{
			if (GUILayout.Button("Set Player's ISP"))
				devMenu.PushMenu(new SetPlayerIspMenu());
			
			if (GUILayout.Button("Create ISP"))
				devMenu.PushMenu(new CreateIspMenu(world));

			if (GUILayout.Button("Create LAN"))
				devMenu.PushMenu(new CreateLanMenu(world));
			
			if (GUILayout.Button("Create Computer"))
				devMenu.PushMenu(new CreateComputerMenu(world));
			
			if (GUILayout.Button("Set LAN for Computer"))
				devMenu.PushMenu(new SetComputerLANMenu(world));
			
			if (GUILayout.Button("Manage LAN Forwarding Tables"))
				devMenu.PushMenu(new ManageForwardingTablesMenu());
			
			if (GUILayout.Button("Find Public Hosts"))
				devMenu.PushMenu(new FindPublicHostsMenu(world));
		}
	}
}