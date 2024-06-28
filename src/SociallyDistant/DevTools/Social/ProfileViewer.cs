#nullable  enable
using ImGuiNET;
using SociallyDistant.Core;
using SociallyDistant.Core.Core;
using SociallyDistant.Core.Core.WorldData.Data;

namespace SociallyDistant.DevTools.Social
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
				ImGui.Text("Profile doesn't exist");
				return;
			}

			WorldProfileData profile = world.World.Profiles[profileId];

			ImGui.Text($"Gender: {profile.Gender}");
			ImGui.Text($"Private: {profile.IsSocialPrivate}");
			ImGui.Text($"Chat name: {profile.ChatName}");
			ImGui.Text($"Chat username: {profile.ChatUsername}");
			
			ImGui.Text("Bio");
			ImGui.Text(profile.SocialBio);

			ImGui.Text("Relationships");

			var relationships = new List<WorldRelationshipData>();

			foreach (WorldRelationshipData relationship in world.World.Relationships)
			{
				if (relationship.Source != profileId && relationship.Target != profileId)
					continue;
				
				relationships.Add(relationship);
			}

			ImGui.Columns(3);
			ImGui.Text("Profile ID");
			ImGui.NextColumn();
			ImGui.Text("Relationship type");
			ImGui.NextColumn();
			ImGui.Text("Actions");;
			ImGui.NextColumn();
			ImGui.Columns(1);

			if (relationships.Count == 0)
				ImGui.Text("No relationships");
			
			foreach (WorldRelationshipData relationship in relationships)
			{
				ImGui.Columns(2);

				ImGui.Text($"{relationship.Source} -> {relationship.Target}");
				ImGui.NextColumn();
				ImGui.Text($"{relationship.Type}");

				ImGui.Columns(1);

				if (ImGui.Button("View other profile"))
				{
					if (relationship.Source == profileId)
						devMenu.PushMenu(new ProfileViewer(world, relationship.Target));
					else
						devMenu.PushMenu(new ProfileViewer(world, relationship.Source));
				}
				
				if (ImGui.Button("Delete"))
				{
					world.World.Relationships.Remove(relationship);
				}
			}
			
			ImGui.Text("Actions");
			
			if (ImGui.Button("Add relationship"))
				devMenu.PushMenu(new AddRelationshipMenu(world, profileId, relationships));
            
			
			if (ImGui.Button("Create Social Post"))
				devMenu.PushMenu(new CreateSocialPost(world, profileId));
			
			if (profileId != world.World.PlayerData.Value.PlayerProfile)
			{
				if (ImGui.Button("Possess"))
				{
					WorldPlayerData playerDAta = world.World.PlayerData.Value;
					playerDAta.PlayerProfile = profile.InstanceId;
					world.World.PlayerData.Value = playerDAta;
				}
				
				if (ImGui.Button("Delete profile"))
				{
					world.World.Profiles.Remove(profile);
					devMenu.PopMenu();
				}
			}
			else
			{
				ImGui.Text("Deleting the player profile isn't a good idea, even for a developer. Action blocked.");
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
			ImGui.Text($"Profile: {profile}");
			ImGui.Text("Post Content");

			//text = GUILayout.TextArea(text);

			if (ImGui.Button("Create"))
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