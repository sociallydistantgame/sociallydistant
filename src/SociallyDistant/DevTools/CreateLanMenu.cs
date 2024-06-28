#nullable enable
using ImGuiNET;
using SociallyDistant.Core;
using SociallyDistant.Core.Core.WorldData.Data;

namespace SociallyDistant.DevTools
{
	public class CreateLanMenu : IDevMenu
	{
		private WorldManager world;
		private WorldLocalNetworkData network;
		private WorldInternetServiceProviderData[] providers;
		private int ispIndex = -1;
		private bool selectingIsp;

		public string Name => "Create LAN";
		
		public CreateLanMenu(WorldManager world)
		{
			this.world = world;
			this.providers = world.World.InternetProviders.ToArray();
		}

		public void OnMenuGUI(DeveloperMenu devMenu)
		{
			if (selectingIsp)
			{
				for (var i = 0; i < providers.Length; i++)
				{
					if (!ImGui.Button(providers[i].Name))
						continue;

					ispIndex = i;
					selectingIsp = false;
					break;
				}
				
				return;
			}
			
			ImGui.Text("Name:");
			//network.Name = GUILayout.TextField(network.Name);
			
			ImGui.Text("Internet Service Provider:");
			
			if (ispIndex == -1)
				ImGui.Text("None");
			else
				ImGui.Text(providers[ispIndex].Name);

			if (ImGui.Button("Select"))
				selectingIsp = true;
			
			if (ispIndex == -1)
				return;

			if (!ImGui.Button("Create"))
				return;

			network.InstanceId = world.GetNextObjectId();
			network.ServiceProviderId = providers[ispIndex].InstanceId;
			world.World.LocalAreaNetworks.Add(network);
			
			devMenu.PopMenu();
		}
	}
}