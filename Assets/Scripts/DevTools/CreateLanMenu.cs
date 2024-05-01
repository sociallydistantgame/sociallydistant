#nullable enable
using Core;
using Core.WorldData.Data;
using UnityEngine;

namespace DevTools
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
					if (!GUILayout.Button(providers[i].Name))
						continue;

					ispIndex = i;
					selectingIsp = false;
					break;
				}
				
				return;
			}
			
			GUILayout.BeginHorizontal();
			GUILayout.Label("Name:");
			GUILayout.FlexibleSpace();
			network.Name = GUILayout.TextField(network.Name);
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label("Internet Service Provider:");
			GUILayout.FlexibleSpace();

			if (ispIndex == -1)
				GUILayout.Label("None");
			else
				GUILayout.Label(providers[ispIndex].Name);

			if (GUILayout.Button("Select"))
				selectingIsp = true;
			
			GUILayout.EndHorizontal();

			if (ispIndex == -1)
				return;

			if (!GUILayout.Button("Create"))
				return;

			network.InstanceId = world.GetNextObjectId();
			network.ServiceProviderId = providers[ispIndex].InstanceId;
			world.World.LocalAreaNetworks.Add(network);
			
			devMenu.PopMenu();
		}
	}
}