#nullable enable
using Core;
using Core.WorldData.Data;
using UnityEngine;
using UnityEngine.Analytics;

namespace DevTools.Social
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
			profile.IsSocialPrivate = GUILayout.Toggle(profile.IsSocialPrivate, "Is this a private profile?");
			
			GUILayout.Label($"Gender: {profile.Gender}");

			GUILayout.BeginHorizontal();

			if (GUILayout.Button("Male"))
				profile.Gender = Gender.Male;
			if (GUILayout.Button("Female"))
				profile.Gender = Gender.Female;
			if (GUILayout.Button("Non-binary"))
				profile.Gender = Gender.Unknown;
			
			GUILayout.EndHorizontal();

			GUILayout.Label("Usernames");

			GUILayout.BeginHorizontal();

			GUILayout.Label("Chat");
			profile.ChatUsername = GUILayout.TextField(profile.ChatUsername);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Email");
			profile.MailUsername = GUILayout.TextField(profile.MailUsername);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Social Media");
			profile.SocialUsername = GUILayout.TextField(profile.SocialUsername);
			GUILayout.EndHorizontal();

			GUILayout.Label("Display Names");
			
			GUILayout.BeginHorizontal();
			GUILayout.Label("Chat");
			profile.ChatName = GUILayout.TextField(profile.ChatName);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Email");
			profile.MailName = GUILayout.TextField(profile.MailName);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Social Media");
			profile.SocialName = GUILayout.TextField(profile.SocialName);
			GUILayout.EndHorizontal();
			
			GUILayout.Label("Social Bio");
			profile.SocialBio = GUILayout.TextArea(profile.SocialBio);

			GUILayout.Label("Friends, followers, and block lists can be managed in the Profile Viewer on the next screen.");
			
			if (GUILayout.Button("Create"))
			{
				profile.InstanceId = world.GetNextObjectId();
				world.World.Profiles.Add(profile);
				devMenu.PopMenu();
                
				devMenu.PushMenu(new ProfileViewer(world, profile.InstanceId));
			}
		}
	}
}