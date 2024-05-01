#nullable enable
using Core;
using Core.WorldData.Data;
using UnityEngine;

namespace DevTools
{
	public class SetPlayerIspMenu : IDevMenu
	{
		private WorldManager world;
		private WorldInternetServiceProviderData[] providers;
		
		/// <inheritdoc />
		public string Name => "Set Player ISP menu";

		public SetPlayerIspMenu(WorldManager world)
		{
			this.world = world;
			this.providers = this.world.World.InternetProviders.ToArray();
		}
		
		/// <inheritdoc />
		public void OnMenuGUI(DeveloperMenu devMenu)
		{
			for (var i = 0; i < providers.Length; i++)
			{
				if (!GUILayout.Button(providers[i].Name)) 
					continue;
				
				WorldPlayerData playerData = this.world.World.PlayerData.Value;
				playerData.PlayerInternetProvider = providers[i].InstanceId;
				world.World.PlayerData.Value = playerData;

				devMenu.PopMenu();
			}
		}
	}
}