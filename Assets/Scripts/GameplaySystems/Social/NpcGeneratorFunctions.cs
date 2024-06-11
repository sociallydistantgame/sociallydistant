#nullable enable
using System;
using System.Linq;
using ContentManagement;
using Core;
using Core.Scripting;
using Core.WorldData.Data;
using Modules;
using UnityEngine;
using UnityEngine.Analytics;

namespace GameplaySystems.Social
{
	public sealed class NpcGeneratorFunctions : ScriptModule
	{
		private readonly IWorldManager world;
		
		private WorldProfileData profile;
		private bool pendingChanges;

		public NpcGeneratorFunctions(IWorldManager world, string target)
		{
			this.world = world;
			ChangeNpc(target);
		}
		
		private void ChangeNpc(string narrativeId)
		{
			if (narrativeId == "player") 
				throw new InvalidOperationException("S character generation script tried to modify the player's profile.");
            
			SavePendingChanges();

			profile = world.World.Profiles.GetNarrativeObject(narrativeId);
		}

		[Function("username")]
		public void SetUsername(string username)
		{
			ThrowIfInvalidId();
			profile.ChatUsername = username;
			pendingChanges = true;
		}

		[Function("displayname")]
		public void SetDisplayName(string[] args)
		{
			ThrowIfInvalidId();
			profile.ChatName = string.Join(" ", args);
			pendingChanges = true;
		}
		
		[Function("bio")]
		public void SetBio(string[] args)
		{
			ThrowIfInvalidId();
			profile.SocialBio = string.Join(" ", args);
			pendingChanges = true;
		}

		[Function("profile")]
		public void SetPrivacy(string value)
		{
			ThrowIfInvalidId();
			pendingChanges = true;

			profile.IsSocialPrivate = value.ToLower().Trim() switch
			{
				"public" => false,
				"private" => true,
				_ => throw new InvalidOperationException($"'{value}' is not a valid profile privacy type")
			};
		}
		
		[Function("pronoun")]
		public void SetPronoun(string pronoun)
		{
			if (!Enum.TryParse(pronoun, true, out ScriptPronoun pronounValue))
				throw new InvalidOperationException($"'{pronoun}' is not a valid pronoun identifier.");

			ThrowIfInvalidId();

			pendingChanges = true;
			profile.Gender = pronounValue switch
			{
				ScriptPronoun.Male => Gender.Male,
				ScriptPronoun.Female => Gender.Female,
				ScriptPronoun.Enby => Gender.Unknown,
				_ => Gender.Unknown
			};
		}
		
		private void ThrowIfInvalidId()
		{
			if (profile.InstanceId.IsInvalid)
				throw new InvalidOperationException("An NPC generation script is trying to generate a profile with an invalid ID.");
		}
		
		public void SavePendingChanges()
		{
			if (!pendingChanges)
				return;

			ThrowIfInvalidId();
			
			world.World.Profiles.Modify(profile);
			pendingChanges = false;
		}

		[Function("friend")]
		public void Friend(string id)
		{
			ThrowIfInvalidId();
			AddRelationship(id, RelationshipType.Friend);
		}
		
		[Function("unfriend")]
		public void Unfriend(string id)
		{
			ThrowIfInvalidId();
			RemoveRelationship(id, RelationshipType.Friend);
		}
		
		[Function("follow")]
		public void Follow(string id)
		{
			ThrowIfInvalidId();
			AddRelationship(id, RelationshipType.Follow);
		}
		
		[Function("unfollow")]
		public void Unfollow(string id)
		{
			ThrowIfInvalidId();
			RemoveRelationship(id, RelationshipType.Follow);
		}
		
		[Function("block")]
		public void Block(string id)
		{
			ThrowIfInvalidId();
			AddRelationship(id, RelationshipType.Blocked);
		}
		
		[Function("unblock")]
		public void Unblock(string id)
		{
			ThrowIfInvalidId();
			RemoveRelationship(id, RelationshipType.Blocked);
		}
		
		
		
		private void AddRelationship(string other, RelationshipType type)
		{
			ObjectId self = profile.InstanceId;
			ObjectId target = ResolveNarrativeId(other);

			WorldRelationshipData relationship = world.World.Relationships.FirstOrDefault(x => x.Type == type && x.Source == self && x.Target == target);

			if (relationship.InstanceId.IsInvalid)
			{
				relationship.InstanceId = world.GetNextObjectId();
				relationship.Type = type;
				relationship.Target = target;
				relationship.Source = self;
				
				world.World.Relationships.Add(relationship);
			}
		}
		
		private void RemoveRelationship(string other, RelationshipType type)
		{
			ObjectId self = profile.InstanceId;
			ObjectId target = ResolveNarrativeId(other);

			WorldRelationshipData relationship = world.World.Relationships.FirstOrDefault(x => x.Type == type && x.Source == self && x.Target == target);

			if (!relationship.InstanceId.IsInvalid)
			{
				world.World.Relationships.Remove(relationship);
			}
		}
		
		private ObjectId ResolveNarrativeId(string id)
		{
			if (id == "player")
				return world.World.PlayerData.Value.PlayerProfile;

			return world.World.Profiles.GetNarrativeObject(id).InstanceId;
		}
		
		private enum ScriptPronoun
		{
			Male,
			Female,
			Enby
		}
	}
}