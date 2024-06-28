#nullable enable
using SociallyDistant.Core.Chat;
using SociallyDistant.Core.Core;
using SociallyDistant.Core.Core.WorldData.Data;
using SociallyDistant.Core.Social;

namespace SociallyDistant.GameplaySystems.Chat
{
	[Serializable]
	public sealed class ConversationScriptCondition
	{
		
		private ScriptConditionType meetType;

		
		private ScriptConditionCheck checkType;

		
		private string[] parameters = Array.Empty<string>();

		public ScriptConditionType Type
		{
			get => meetType;
			set => meetType = value;
		}

		public ScriptConditionCheck Check
		{
			get => checkType;
			set => checkType = value;
		}

		public string[] Parameters
		{
			get => parameters;
			set => parameters = value;
		}

		private bool CheckConditionInternal(IWorldManager world, ISocialService socialService)
		{
			IProfile player = socialService.PlayerProfile;
			
			// TODO: Actually make these work.
			switch (checkType)
			{
				case ScriptConditionCheck.MissionCompleted:
					return world.World.IsMissionCompleted(Parameters[0]);
				case ScriptConditionCheck.InteractionCompleted:
					return world.World.IsInteractionCompleted(Parameters[0]);
				case ScriptConditionCheck.IsInMission:
					return world.World.CurrentMissionId == Parameters[0];
				case ScriptConditionCheck.FriendsWith:
				{
					IProfile friend = socialService.GetNarrativeProfile(Parameters[0]);
					return player.IsFriendsWith(friend);
				}
				case ScriptConditionCheck.BlockedBy:
				{
					IProfile user = socialService.GetNarrativeProfile(Parameters[0]);
					return player.IsBlockedBy(user);
				}
				case ScriptConditionCheck.SeenProfile:
				{
					IProfile user = socialService.GetNarrativeProfile(Parameters[0]);
					return world.World.WitnessedObjects.Any(x => x.Type == WitnessedObjectType.Profile && x.WitnessedObject == user.ProfileId);
				}
				case ScriptConditionCheck.SeenNetwork:
				{
					// Find an existing narrative network
					WorldLocalNetworkData network = world.World.LocalAreaNetworks.FirstOrDefault(x => x.NarrativeId == Parameters[0]);
					if (network.InstanceId.IsInvalid)
						return false;

					return world.World.WitnessedObjects.Any(x => x.Type == WitnessedObjectType.Network && x.WitnessedObject == network.InstanceId);
				}
				case ScriptConditionCheck.SeenDevice:
				{
					WorldComputerData device = world.World.Computers.FirstOrDefault(x => x.NarrativeId == Parameters[0]);
					if (device.InstanceId.IsInvalid)
						return false;

					return world.World.WitnessedObjects.Any(x => x.Type == WitnessedObjectType.Device && x.WitnessedObject == device.InstanceId);
				}
				case ScriptConditionCheck.ReachedLevel:
				{
					if (!int.TryParse(Parameters[0], out int requiredLevel))
						return false;
					
					PlayerLevelInfo levelInfo = SociallyDistantUtility.GetPlayerLevelFromExperience(world.World.PlayerExperience);

					return levelInfo.Level >= requiredLevel;
				}
				case ScriptConditionCheck.WorldFlag:
					return world.World.WorldFlags.Contains(Parameters[0]);
				case ScriptConditionCheck.Lifepath:
					return world.World.NarrativeLifePath == Parameters[0];
				case ScriptConditionCheck.MissionFailed:
					return world.World.WasMissionFailed(Parameters[0]);
				default:
					throw new ArgumentOutOfRangeException();
			}
			
			return true;
		}
		
		public bool CheckCondition(IWorldManager world, ISocialService socialService)
		{
			bool checkResult = CheckConditionInternal(world, socialService);
			return meetType == ScriptConditionType.Met ? checkResult : !checkResult;
		}
	}
}