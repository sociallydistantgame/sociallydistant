#nullable enable
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.WorldData.Data;
using UnityEngine;
using UnityEngine.Profiling;

namespace DevTools.Social
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
				GUILayout.Label("Select relationship type");

				if (GUILayout.Button("Friend"))
				{
					type = RelationshipType.Friend;
					blockedIds.Clear();
					blockedIds.AddRange(existingRelationships.
						Where(x=>x.Type==type).
						Select(x => x.Target));
					selectedTypeYet = true;
				}
				
				if (GUILayout.Button("Follow"))
				{
					type = RelationshipType.Follow;
					blockedIds.Clear();
					blockedIds.AddRange(existingRelationships.
						Where(x=>x.Type==type).
						Select(x => x.Target));
					selectedTypeYet = true;
				}
				
				if (GUILayout.Button("Blocked"))
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

			GUILayout.BeginHorizontal();

			GUILayout.Label($"Relationship type: {type}");
			
			if (GUILayout.Button("Change"))
				selectedTypeYet = false;
            
			GUILayout.EndHorizontal();

			GUILayout.Label("Select profile");

			foreach (WorldProfileData profile in world.World.Profiles)
			{
				if (profile.InstanceId == sourceProfileId)
					continue;

				if (blockedIds.Contains(profile.InstanceId))
					continue;

				if (GUILayout.Button($"{profile.InstanceId}: {profile.ChatName}"))
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