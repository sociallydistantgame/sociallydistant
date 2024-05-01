#nullable  enable
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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
            
			
			if (GUILayout.Button("Create Social Post"))
				devMenu.PushMenu(new CreateSocialPost(world, profileId));
			
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

	public class CreateSocialPost : IDevMenu
	{
		private readonly WorldManager worldManager;
		private readonly ObjectId profile;
		private WorldPostData post;
		private string text = string.Empty;

		public CreateSocialPost(WorldManager world, ObjectId profile)
		{
			this.worldManager = world;
			this.profile = profile;
		}

		/// <inheritdoc />
		public string Name => "Create Social Post";

		/// <inheritdoc />
		public void OnMenuGUI(DeveloperMenu devMenu)
		{
			GUILayout.Label($"Profile: {profile}");
			GUILayout.Label("Post Content");

			text = GUILayout.TextArea(text);

			if (GUILayout.Button("Create"))
			{
				post.InstanceId = worldManager.GetNextObjectId();
				post.Author = profile;
				post.Date = DateTime.UtcNow;
				post.DocumentElements = new DocumentElement[]
				{
					new DocumentElement
					{
						ElementType = DocumentElementType.Text,
						Data = text
					}
				};

				worldManager.World.Posts.Add(post);
				
				devMenu.PopMenu();
			}
		}
	}
}