using Core;
using Core.WorldData;
using UnityEngine;

namespace DevTools.Social
{
	public class ProfileDebug : IDevMenu
	{
		private readonly WorldManager world;
			
		public ProfileDebug(WorldManager world)
		{
			this.world = world;
		}

		/// <inheritdoc />
		public string Name => "Social Profiles";

		/// <inheritdoc />
		public void OnMenuGUI(DeveloperMenu devMenu)
		{
			GUILayout.Label("General Actions");
			if (GUILayout.Button("Create New Profile"))
				devMenu.PushMenu(new CreateProfileMenu(world));
			
			ObjectId playerId = world.World.PlayerData.Value.PlayerProfile;

			if (world.World.Profiles.ContainsId(playerId))
			{
				GUILayout.Label("Player");;
				if (GUILayout.Button("Player Profile"))
					devMenu.PushMenu(new ProfileViewer(world, playerId));
			}
            
			GUILayout.Label("All Profiles");
			
			foreach (var profile in world.World.Profiles)
			{
				if (GUILayout.Button($"{profile.InstanceId}: {profile.ChatName}"))
					devMenu.PushMenu(new ProfileViewer(world, profile.InstanceId));
			}
		}
	}
}