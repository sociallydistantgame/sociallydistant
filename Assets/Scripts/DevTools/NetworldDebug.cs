#nullable enable
using Core;
using Core.WorldData;
using GameplaySystems.Networld;
using Player;

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
		public void OnGUI()
		{
		}
	}
}