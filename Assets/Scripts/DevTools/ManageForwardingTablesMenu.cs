#nullable enable
using Core;
using Core.WorldData.Data;
using UnityEngine;

namespace DevTools
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
			GUILayout.Label("Select LAN to edit");

			for (var i = 0; i < lans.Length; i++)
			{
				if (!GUILayout.Button(lans[i].Name))
					continue;

				devMenu.PushMenu(new ManageLanRulesMenu(world, lans[i]));
			}
		}
	}
}