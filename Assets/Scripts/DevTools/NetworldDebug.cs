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

		public NetworldDebug(WorldManager world, NetworkSimulationController simulation, PlayerInstanceHolder playerInstance)
		{
			this.world = world;
			this.simulation = simulation;
			this.player = playerInstance;
		}
		
		/// <inheritdoc />
		public void OnMenuGUI(DeveloperMenu devMenu)
		{
			if (GUILayout.Button("Create ISP"))
				devMenu.PushMenu(new CreateIspMenu(world));
			
			if (GUILayout.Button("Create Computer"))
				devMenu.PushMenu(new CreateComputerMenu(world));
		}
	}
}