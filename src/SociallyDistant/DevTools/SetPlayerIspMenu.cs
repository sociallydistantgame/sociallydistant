#nullable enable
using ImGuiNET;
using SociallyDistant.Core;
using SociallyDistant.Core.Core.WorldData.Data;

namespace SociallyDistant.DevTools
{
	public class SetPlayerIspMenu : IDevMenu
	{
		private WorldManager world;
		private WorldInternetServiceProviderData[] providers;
		
		/// <inheritdoc />
		public string Name => "Set Player ISP menu";

		public SetPlayerIspMenu()
		{
			this.world = WorldManager.Instance;
			this.providers = this.world.World.InternetProviders.ToArray();
		}
		
		/// <inheritdoc />
		public void OnMenuGUI(DeveloperMenu devMenu)
		{
			for (var i = 0; i < providers.Length; i++)
			{
				if (!ImGui.Button(providers[i].Name)) 
					continue;
				
				WorldPlayerData playerData = this.world.World.PlayerData.Value;
				playerData.PlayerInternetProvider = providers[i].InstanceId;
				world.World.PlayerData.Value = playerData;

				devMenu.PopMenu();
			}
		}
	}
}