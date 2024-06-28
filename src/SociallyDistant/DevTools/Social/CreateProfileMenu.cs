#nullable enable
using ImGuiNET;
using SociallyDistant.Core;
using SociallyDistant.Core.Core;
using SociallyDistant.Core.Core.WorldData.Data;

namespace SociallyDistant.DevTools.Social
{
	public class CreateProfileMenu : IDevMenu
	{
		private readonly WorldManager world;
		private WorldProfileData profile;
        
		public CreateProfileMenu(WorldManager world)
		{
			this.world = world;
		}

		/// <inheritdoc />
		public string Name => "Create Social Profile";

		/// <inheritdoc />
		public void OnMenuGUI(DeveloperMenu devMenu)
		{
			bool isPrivate = profile.IsSocialPrivate;
			if (ImGui.Checkbox("Is profile private", ref isPrivate))
				profile.IsSocialPrivate = isPrivate;
			
			ImGui.Text($"Gender: {profile.Gender}");
			
			if (ImGui.Button("Male"))
				profile.Gender = Gender.Male;
			if (ImGui.Button("Female"))
				profile.Gender = Gender.Female;
			if (ImGui.Button("Non-binary"))
				profile.Gender = Gender.Unknown;
			
			ImGui.Text("Usernames");
			
			ImGui.Text("Chat");
			//profile.ChatUsername = GUILayout.TextField(profile.ChatUsername);
			ImGui.Text("Email");
			ImGui.Text("Social Media");

			ImGui.Text("Display Names");
			
			ImGui.Text("Chat");
			//profile.ChatName = GUILayout.TextField(profile.ChatName);
			ImGui.Text("Email");
			ImGui.Text("Social Media");
			
			ImGui.Text("Social Bio");
			//profile.SocialBio = GUILayout.TextArea(profile.SocialBio);

			ImGui.Text("Friends, followers, and block lists can be managed in the Profile Viewer on the next screen.");
			
			if (ImGui.Button("Create"))
			{
				profile.InstanceId = world.GetNextObjectId();
				world.World.Profiles.Add(profile);
				devMenu.PopMenu();
                
				devMenu.PushMenu(new ProfileViewer(world, profile.InstanceId));
			}
		}
	}
}