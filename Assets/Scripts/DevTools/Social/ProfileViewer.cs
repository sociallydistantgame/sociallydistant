#nullable  enable
using System.Collections.Generic;
using Core;
using Core.WorldData.Data;
using UnityEngine;

namespace DevTools.Social
{
	public class ProfileViewer : IDevMenu
	{
		private readonly WorldManager world;
		private readonly ObjectId profileId;

		public ProfileViewer(WorldManager world, ObjectId id)
		{
			this.world = world;
			this.profileId = id;
		}

		/// <inheritdoc />
		public string Name => "Social Profile Viewer";

		/// <inheritdoc />
		public void OnMenuGUI(DeveloperMenu devMenu)
		{
			if (!world.World.Profiles.ContainsId(profileId))
			{
				GUILayout.Label("Profile doesn't exist");
				return;
			}

			WorldProfileData profile = world.World.Profiles[profileId];

			GUILayout.Label($"Gender: {profile.Gender}");
			GUILayout.Label($"Private: {profile.IsSocialPrivate}");
			GUILayout.Label($"Chat name: {profile.ChatName}");
			GUILayout.Label($"Chat username: {profile.ChatUsername}");
			GUILayout.Label($"Mail name: {profile.MailName}");
			GUILayout.Label($"Mail username: {profile.MailUsername}");
			GUILayout.Label($"Social name: {profile.SocialName}");
			GUILayout.Label($"Social username: {profile.SocialUsername}");

			GUILayout.Label("Bio");
			GUILayout.Label(profile.SocialBio);

			GUILayout.Label("Relationships");

			var relationships = new List<WorldRelationshipData>();

			foreach (WorldRelationshipData relationship in world.World.Relationships)
			{
				if (relationship.Source != profileId && relationship.Target != profileId)
					continue;
				
				relationships.Add(relationship);
			}

			GUILayout.BeginHorizontal();
			GUILayout.Label("Profile ID");
			GUILayout.Label("Relationship type");
			GUILayout.Label("Actions");;
			GUILayout.EndHorizontal();

			if (relationships.Count == 0)
				GUILayout.Label("No relationships");
			
			foreach (WorldRelationshipData relationship in relationships)
			{
				GUILayout.BeginHorizontal();

				GUILayout.Label($"{relationship.Source} -> {relationship.Target}");
				GUILayout.Label($"{relationship.Type}");

				GUILayout.BeginVertical();

				if (GUILayout.Button("View other profile"))
				{
					if (relationship.Source == profileId)
						devMenu.PushMenu(new ProfileViewer(world, relationship.Target));
					else
						devMenu.PushMenu(new ProfileViewer(world, relationship.Source));
				}
				
				if (GUILayout.Button("Delete"))
				{
					world.World.Relationships.Remove(relationship);
				}
				
				GUILayout.EndVertical();
				
				GUILayout.EndHorizontal();
			}
			
			GUILayout.Label("Actions");
			
			if (GUILayout.Button("Add relationship"))
				devMenu.PushMenu(new AddRelationshipMenu(world, profileId, relationships));
            
			
			if (profileId != world.World.PlayerData.Value.PlayerProfile)
			{
				if (GUILayout.Button("Possess"))
				{
					WorldPlayerData playerDAta = world.World.PlayerData.Value;
					playerDAta.PlayerProfile = profile.InstanceId;
					world.World.PlayerData.Value = playerDAta;
				}
				
				if (GUILayout.Button("Delete profile"))
				{
					world.World.Profiles.Remove(profile);
					devMenu.PopMenu();
				}
			}
			else
			{
				GUILayout.Label("Deleting the player profile isn't a good idea, even for a developer. Action blocked.");
			}
		}
	}
}