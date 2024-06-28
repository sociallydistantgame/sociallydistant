#nullable enable
using ImGuiNET;
using SociallyDistant.Core;
using SociallyDistant.Core.Core;
using SociallyDistant.Core.Core.WorldData.Data;

namespace SociallyDistant.DevTools.Social
{
	public class AddRelationshipMenu : IDevMenu
	{
		private readonly WorldManager world;
		private readonly ObjectId sourceProfileId;
		private readonly IReadOnlyList<WorldRelationshipData> existingRelationships;
		private readonly List<ObjectId> blockedIds = new List<ObjectId>();
		private bool selectedTypeYet;
		private RelationshipType type;
		
		/// <inheritdoc />
		public string Name => "Add relationship";

		public AddRelationshipMenu(WorldManager world, ObjectId source, IReadOnlyList<WorldRelationshipData> existingRelationships)
		{
			this.world = world;
			this.sourceProfileId = source;
			this.existingRelationships = existingRelationships;
		}

		/// <inheritdoc />
		public void OnMenuGUI(DeveloperMenu devMenu)
		{
			if (!selectedTypeYet)
			{
				ImGui.Text("Select relationship type");

				if (ImGui.Button("Friend"))
				{
					type = RelationshipType.Friend;
					blockedIds.Clear();
					blockedIds.AddRange(existingRelationships.
						Where(x=>x.Type==type).
						Select(x => x.Target));
					selectedTypeYet = true;
				}
				
				if (ImGui.Button("Follow"))
				{
					type = RelationshipType.Follow;
					blockedIds.Clear();
					blockedIds.AddRange(existingRelationships.
						Where(x=>x.Type==type).
						Select(x => x.Target));
					selectedTypeYet = true;
				}
				
				if (ImGui.Button("Blocked"))
				{
					type = RelationshipType.Blocked;
					blockedIds.Clear();
					blockedIds.AddRange(existingRelationships.
						Where(x=>x.Type==type).
						Select(x => x.Target));
					selectedTypeYet = true;
				}
				
				return;
			}
			
			ImGui.Text($"Relationship type: {type}");
			
			if (ImGui.Button("Change"))
				selectedTypeYet = false;
			
			ImGui.Text("Select profile");

			foreach (WorldProfileData profile in world.World.Profiles)
			{
				if (profile.InstanceId == sourceProfileId)
					continue;

				if (blockedIds.Contains(profile.InstanceId))
					continue;

				if (ImGui.Button($"{profile.InstanceId}: {profile.ChatName}"))
				{
					world.World.Relationships.Add(new WorldRelationshipData
					{
						InstanceId = world.GetNextObjectId(),
						Type = type,
						Source = sourceProfileId,
						Target = profile.InstanceId
					});
					
					devMenu.PopMenu();
					return;
				}
			}
		}
	}
}