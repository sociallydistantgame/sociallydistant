using ImGuiNET;
using SociallyDistant.Core;
using SociallyDistant.Core.Core;

namespace SociallyDistant.DevTools.Social
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
			ImGui.Text("General Actions");
			if (ImGui.Button("Create New Profile"))
				devMenu.PushMenu(new CreateProfileMenu(world));
			
			ObjectId playerId = world.World.PlayerData.Value.PlayerProfile;

			if (world.World.Profiles.ContainsId(playerId))
			{
				ImGui.Text("Player");;
				if (ImGui.Button("Player Profile"))
					devMenu.PushMenu(new ProfileViewer(world, playerId));
			}
            
			ImGui.Text("All Profiles");
			
			foreach (var profile in world.World.Profiles)
			{
				if (ImGui.Button($"{profile.InstanceId}: {profile.ChatName}"))
					devMenu.PushMenu(new ProfileViewer(world, profile.InstanceId));
			}
		}
	}
}