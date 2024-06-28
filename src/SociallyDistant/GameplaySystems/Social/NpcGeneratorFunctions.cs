#nullable enable
using System.Diagnostics;
using Serilog;
using SociallyDistant.Core.Core;
using SociallyDistant.Core.Core.Scripting;
using SociallyDistant.Core.Core.WorldData.Data;

namespace SociallyDistant.GameplaySystems.Social
{
	public sealed class NpcGeneratorFunctions : ScriptModule
	{
		private readonly IWorldManager world;
		
		private WorldProfileData profile;
		private bool pendingChanges;

		private bool posting;
		private bool postChanged;
		private WorldPostData post;
		
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

		[Function("attributes")]
		private void SetCharacterAttributes(string[] attribNames)
		{
			ThrowIfInvalidId();

			var newAttributes = CharacterAttributes.None;

			foreach (string attribName in attribNames)
			{
				if (!Enum.TryParse(attribName, true, out CharacterAttributes attrib))
					throw new InvalidOperationException($"Unrecognized character attribute name {attribName} for NPC {this.profile.NarrativeId}");

				newAttributes |= attrib;
			}

			pendingChanges = profile.Attributes != newAttributes;
			profile.Attributes = newAttributes;
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
			if (posting)
			{
				Log.Warning("A social post was being created during an NPC update, but the post was never sent.");
				SendPost();
			}

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

		[Function("post")]
		private void StartPost(string id)
		{
			ThrowIfPosting();

			string fullId = $"{profile.NarrativeId}:{id}";

			post = world.World.Posts.GetNarrativeObject(fullId);

			if (post.Author != profile.InstanceId)
			{
				post.Author = profile.InstanceId;
				postChanged = true;
			}

			posting = true;
		}

		[Function("body")]
		private void SetPostText(string[] textData)
		{
			ThrowIfNotPosting();

			string text = string.Join(" ", textData);
			
			List<DocumentElement> document = post.DocumentElements?.ToList() ?? new List<DocumentElement>();

			if (document.Count == 0)
			{
				document.Add(new DocumentElement
				{
					ElementType = DocumentElementType.Text,
					Data = text
				});
			}
			else
			{
				// Find an existing text element. If we can't find one, then we insert it at index 0.
				var existingIndex = -1;

				for (var i = 0; i < document.Count; i++)
				{
					if (document[i].ElementType == DocumentElementType.Text)
					{
						existingIndex = i;
						break;
					}
				}

				if (existingIndex == -1)
				{
					document.Insert(0, new DocumentElement
					{
						ElementType = DocumentElementType.Text,
						Data = text
					});
				}
				else
				{
					// Swap it with item 0 and update
					DocumentElement element = document[existingIndex];
					document[existingIndex] = document[0];

					element.Data = text;
					
					document[0] = element;
				}
			}

			postChanged |= post.DocumentElements?.SequenceEqual(document) == false;
			post.DocumentElements = document;
		}

		[Function("send")]
		private void SendPost()
		{
			ThrowIfNotPosting();

			posting = false;
			
			if (postChanged)
				world.World.Posts.Modify(post);

			postChanged = false;
			post = default;
		}

		private void ThrowIfNotPosting()
		{
			if (!posting)
				throw new InvalidOperationException("Cannot perform this operation unless creating a social post.");
		}
		
		private void ThrowIfPosting()
		{
			if (posting)
				throw new InvalidOperationException("Cannot start a new post when already creating a social post. End the current post with `send` first.");
		}
		
		private enum ScriptPronoun
		{
			Male,
			Female,
			Enby
		}
	}
}